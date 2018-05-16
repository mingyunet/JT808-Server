using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Model
{
    /// <summary>
    /// 平台应答类
    /// </summary>
    public abstract class PlatformResp
    {
        /// <summary>
        /// 终端ID
        /// </summary>
        public string TerminalId { get; set; }

        /// <summary>
        /// 应答流水号
        /// </summary>
        public short ReplySerial { get; set; }


        public abstract short GetMessageId();
    }
}
