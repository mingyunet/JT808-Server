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
    }
}
