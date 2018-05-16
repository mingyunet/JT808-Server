using DataGateway.GPS.JT808.Megssages;
using DataGateway.GPS.JT808.Messages;
using DotNetty.Buffers;
using JT808DataServer.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Codec
{
    public class JT808ProtoDecoder
    {
        public static byte FLAG_BYTE = 0x7e;


        /// <summary>
        /// 解码字节数组为JT808Message
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static JT808Message Decode(byte[] bytes)
        {
            // Encoding.GetEncoding("GBK").GetBytes(value);

            ByteBuffer buffer = ByteBuffer.Wrap(bytes);
            byte headFlag = buffer.Get();
            buffer.Position = buffer.Capacity - 1;
            byte tailFlag = buffer.Get();

            // --> Check head flag 检查消息头/尾
            if (headFlag != FLAG_BYTE)
            {
                throw new Exception(
                        "Parameter: buffer head flag byte is not " + FLAG_BYTE);
            }
            if (tailFlag != FLAG_BYTE)
            {
                throw new Exception(
                        "Parameter: buffer tail flag byte is not " + FLAG_BYTE);
            }

            buffer.Position = 1;
            buffer.Limit = buffer.Capacity - 1;

            byte[] dataWithoutFlag = new byte[buffer.Capacity - 2];
            buffer.Read(dataWithoutFlag);

            // unescape - 反转义
            byte[] dataAfterUnescape = Unescape(dataWithoutFlag);

            // Validate checkCode - 验证校验码
            byte checkCode = dataAfterUnescape[dataAfterUnescape.Length - 1];

            byte[] dataToCheck = new byte[dataAfterUnescape.Length - 1];
            Array.Copy(dataAfterUnescape, 0, dataToCheck, 0, dataAfterUnescape.Length - 1);


            byte expectCheckCode = CheckCode(dataToCheck);
            if (checkCode != expectCheckCode)
            {
                throw new Exception(
                        "Parameter: buffer check failed, expect-check-code="
                                + expectCheckCode + ", actually-check-code=" + checkCode);
            }

            ByteBuffer dataAfterUnescapeBuffer = ByteBuffer.Wrap(dataAfterUnescape);
            JT808Message message = new JT808Message();

            // Decode head ----------------------------------- Begin
            // 开始头部解码
            JT808MessageHead head = new JT808MessageHead();
            // Message ID - 消息ID
            head.MessageId = dataAfterUnescapeBuffer.GetShort();

            // Message body attributes - 消息体属性
            head.MsgBodyAttr = Decode(dataAfterUnescapeBuffer.GetShort());

            // Terminal SimNo - 终端手机号
            byte[] mobileBytes = new byte[6];
            dataAfterUnescapeBuffer.Read(mobileBytes);

            head.TerminalId = Bcd6_2mobile(mobileBytes);
            head.MessageSerial = dataAfterUnescapeBuffer.GetShort();

            if (head.MsgBodyAttr.IsSplit)
            {
                MessageSplitInfo splitInfo = new MessageSplitInfo();
                splitInfo.Packages = dataAfterUnescapeBuffer.GetShort();
                splitInfo.PackageNo = dataAfterUnescapeBuffer.GetShort();
                head.SplitInfo = splitInfo;
            }
            message.MsgHeader = head;
            // Decode head ------------------------------------ End
            // 解码头部结束

            // Message body
            // 消息体

            int bodyLength = head.MsgBodyAttr.MsgBodyLength;
            byte[] body = new byte[bodyLength];
            dataAfterUnescapeBuffer.Read(body);
            message.MsgBody = body;

            // Check Code - 校验码
            message.CheckCode = checkCode;
            return message;
        }


        /// <summary>
        /// 6字节的BCD码转为终端手机号字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Bcd6_2mobile(byte[] bytes)
        {

            StringBuilder mobile = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                int n = Bcd2decimal(bytes[i]);
                if (i != 0 && n < 10)
                {
                    mobile.Append("0");
                }
                mobile.Append(Bcd2decimal(bytes[i]));
            }

            return mobile.ToString();
        }


        /// <summary>
        /// 10进制数转BCD码
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static byte Num2BCD(int n)
        {
            return (byte)(((n / 10) << 4) + (n % 10));
        }

        /// <summary>
        /// BCD码转10进制数
        /// </summary>
        /// <param name="bcd"></param>
        /// <returns></returns>
        public static int Bcd2decimal(byte bcd)
        {
            return (bcd >> 4 & 0x0F) * 10 + (bcd & 0x0F);
        }


        /// <summary>
        /// 解码为对象
        /// </summary>
        /// <param name="_2Bytes"></param>
        /// <returns></returns>
        public static JT808MessageBodyAttr Decode(short _2Bytes)
        {

            JT808MessageBodyAttr attr = new JT808MessageBodyAttr();

            // isSpilt ?
            int isSplit = _2Bytes >> 13 & 0x1;
            if (isSplit == 1)
            {
                attr.IsSplit = true;
            }

            // Encrypt type
            int encryptTypeVal = _2Bytes >> 10 & 0x1;
            if (encryptTypeVal == 1)
            {
                attr.EncryptionType = 1;
            }
            else
            {
                attr.EncryptionType = 0;
            }

            // Body length
            int bodyLen = _2Bytes & 0x3ff;
            attr.MsgBodyLength = bodyLen;

            return attr;
        }

        /// <summary>
        /// 反转义，规则如下:
        /// 0x7d 0x02 -> 0x7e
        /// 0x7d 0x01 -> 0x7d
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static byte[] Unescape(byte[] target)
        {

            int resultLen = target.Length;
            foreach (byte b in target)
            {
                if (b == 0x7d)
                {
                    resultLen--;
                }
            }

            ByteBuffer buffer = ByteBuffer.Allocate(resultLen);
            bool lastIs7D = false;
            foreach (byte b in target)
            {
                if (b == 0x7d)
                {
                    lastIs7D = true;
                    continue;
                }

                if (lastIs7D == true)
                {
                    if (b == 0x01)
                    {
                        buffer.Put((byte)0x7d);
                    }
                    else if (b == 0x02)
                    {
                        buffer.Put((byte)0x7e);
                    }
                    lastIs7D = false;
                    continue;
                }

                buffer.Put(b);
            }

            return buffer.Array();
        }

        /// <summary>
        /// 计算校验码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte CheckCode(byte[] data)
        {

            byte checkCode = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (i == 0)
                {
                    checkCode = data[i];
                    continue;
                }
                checkCode ^= data[i];
            }
            return checkCode;
        }


        /// <summary>
        /// 按照JT/T808协议, 对消息进行转义，规则如下:
        /// 0x7e -> 0x7d 0x02
        ///  0x7d -> 0x7d 0x01
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static byte[] Escape(byte[] target)
        {

            int resultLen = 0;
            foreach (byte b in target)
            {
                if (b == 0x7e || b == 0x7d)
                {
                    resultLen += 2;
                    continue;
                }
                resultLen++;
            }

            ByteBuffer buffer = ByteBuffer.Allocate(resultLen);
            foreach (byte b in target)
            {
                if (b == 0x7e)
                {
                    buffer.Put((byte)0x7d);
                    buffer.Put((byte)0x02);
                    continue;
                }
                if (b == 0x7d)
                {
                    buffer.Put((byte)0x7d);
                    buffer.Put((byte)0x01);
                    continue;
                }
                buffer.Put(b);
            }

            return buffer.Array();
        } 

        /// <summary>
        /// 手机号字符串转6字节的BCD码 Mobile No. to 6 bytes BCD, mobile 12 chars
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static byte[] Mobile2bcd6(string mobile)
        {
            ByteBuffer buffer = ByteBuffer.Allocate(6);
            long l = long.Parse(mobile);

            buffer.Put(Num2BCD((int)(l / 10000000000L)));
            l %= 10000000000L;
            buffer.Put(Num2BCD((int)(l / 100000000L)));
            l %= 100000000L;
            buffer.Put(Num2BCD((int)(l / 1000000L)));
            l %= 1000000L;
            buffer.Put(Num2BCD((int)(l / 10000L)));
            l %= 10000L;
            buffer.Put(Num2BCD((int)(l / 100L)));
            l %= 100L;
            buffer.Put(Num2BCD((int)l));

            return buffer.Array();
        }

    }
}
