using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Constant
{
    public class MessageResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static byte Success = 0;

        /// <summary>
        /// 失败
        /// </summary>
        public static byte Failure = 1;

        /// <summary>
        /// 消息错误
        /// </summary>
        public static byte Msg_error = 2;

        /// <summary>
        /// 不支持的消息
        /// </summary>
        public static byte Unsupported = 3;

        /// <summary>
        /// Warnning_msg_ack
        /// </summary>
        public static byte Warnning_msg_ack = 4;

    }
}
