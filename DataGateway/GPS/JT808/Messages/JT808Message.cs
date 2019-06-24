using DataGateway.GPS.JT808.Codec;
using DataGateway.GPS.JT808.Constant;
using DotNetty.Transport.Channels;
using JT808DataServer.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Megssages
{
    public class JT808Message
    {
        /// <summary>
        /// 16byte 消息头
        /// </summary>
        public JT808MessageHead MsgHeader { get; set; }

        /// <summary>
        /// 消息体字节数组
        /// </summary>
        public byte[] MsgBody { get; set; }

        /// <summary>
        /// 校验码 1byte
        /// </summary>
        public int CheckCode { get; set; }

        /// <summary>
        /// 将消息按JT/T 808协议进行编码为字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] Encode()
        {
            if (MsgHeader == null)
            {
                throw new Exception("Field: msgHead cannot be null.");
            }

            // 消息头编码
            byte[] msgHeadBytes = MsgHeader.Encode();

            // body
            byte[] msgBodyBytes = null;
            if (MsgBody != null)
            {
                msgBodyBytes = MsgBody;
            }

            // 构建消息（不包含校验码和头尾标识）
            byte[] message = null;
            if (msgBodyBytes != null)
            {
                ByteBuffer msgbuf = ByteBuffer.Allocate(
                        msgHeadBytes.Length + msgBodyBytes.Length);
                msgbuf.Put(msgHeadBytes);
                msgbuf.Put(msgBodyBytes);
                message = msgbuf.Array();
            }
            else
            {
                message = msgHeadBytes;
            }

            // 计算校验码
            byte checkCode = JT808ProtoDecoder.CheckCode(message);
            ByteBuffer checkedBuffer = ByteBuffer.Allocate(message.Length + 1);
            checkedBuffer.Put(message);
            checkedBuffer.Put(checkCode);
            byte[] checkedMessage = checkedBuffer.Array();

            // 转义
            byte[] escapedMessage = JT808ProtoDecoder.Escape(checkedMessage);

            // 增加标识位
            ByteBuffer buffer = ByteBuffer.Allocate(escapedMessage.Length + 2);
            buffer.Put(JT808Constant.PKG_DELIMITER);
            buffer.Put(escapedMessage);
            buffer.Put(JT808Constant.PKG_DELIMITER);

            return buffer.Array();
        }

     
        /// <summary>
        /// 会话正文
        /// </summary>
        public IChannelHandlerContext channel { get; set; }

    }
}
