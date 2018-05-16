using DataGateway.GPS.JT808.Constant;
using JT808DataServer.Common;

namespace DataGateway.GPS.JT808.Model
{
    /// <summary>
    /// 终端注册应答
    /// </summary>
    public class TerminalRegPlatformResp : PlatformResp
    {
        /// <summary>
        ///  响应码
        ///  0：SUCCESS
        ///  1：VEH_REGISTERED
        ///  2：VEH_NOT_FOUND
        ///  3：TERM_registered
        ///  4：TERM_NOT_FOUND
        /// </summary>
        public byte ReplyCode { get; set; }

        /// <summary>
        /// 鉴权码
        /// </summary>
        public string AuthCode { get; set; }

        public override short GetMessageId()
        {
            return (short)JT808Constant.CMD_TERMINAL_REGISTER_RESP;
        }
    }
}
