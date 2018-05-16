using System;
using DataGateway.GPS.JT808.Codec;
using DataGateway.GPS.JT808.Constant;
using DotNetty.Transport.Channels;
using JT808DataServer.Common;

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
        /// 会话正文
        /// </summary>
        public IChannelHandlerContext Channel { get; set; }

    }
}
