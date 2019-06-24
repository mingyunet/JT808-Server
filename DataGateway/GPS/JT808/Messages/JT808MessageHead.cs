using DataGateway.GPS.JT808.Codec;
using DataGateway.GPS.JT808.Constant;
using DataGateway.GPS.JT808.Messages;
using JT808DataServer.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Megssages
{
    /// <summary>
    /// JT/T808协议消息头
    /// </summary>
    public class JT808MessageHead
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public short MessageId { get; set; }

        /// <summary>
        /// 消息体属性
        /// </summary>
        public JT808MessageBodyAttr MsgBodyAttr { get; set; }


        /// <summary>
        /// 终端ID-上传的SIM卡号
        /// </summary>
        public string TerminalId { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public short MessageSerial { get; set; }


        /// <summary>
        /// 消息包封装项
        /// If msgBodyAttr.isSplit=true, then it not null.
        /// </summary>
        public MessageSplitInfo SplitInfo { get; set; }

        /// <summary>
        /// 编码消息头为字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] Encode()
        {

            int capacity = 12;
            byte[] splitInfoBytes = null;
            if (SplitInfo != null)
            {
                splitInfoBytes = SplitInfo.Encode();
                capacity += splitInfoBytes.Length;
            }

            if (MsgBodyAttr == null)
            {
                throw new Exception("Field: msgBodyAttr cannot be null.");
            }

            ByteBuffer buffer = ByteBuffer.Allocate(capacity);
            buffer.PutShort(MessageId);
            buffer.Put(MsgBodyAttr.Encode());

            // mobile
            if (TerminalId.Length  > JT808Constant.MAX_MOBILE_LENGTH)
            {
                throw new Exception(
                        "Field: mobile=" + TerminalId
                                + ", but max allowable value is "
                                + JT808Constant.MAX_MOBILE_LENGTH + ".");
            }
             

            buffer.Put(JT808ProtoDecoder.Mobile2bcd6(TerminalId));
            buffer.PutShort(this.MessageSerial);
            if (splitInfoBytes != null)
            {
                buffer.Put(splitInfoBytes);
            }

            return buffer.Array();
        }


    }
}
