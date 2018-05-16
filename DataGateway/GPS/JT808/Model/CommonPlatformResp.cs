using DataGateway.GPS.JT808.Constant;
using JT808DataServer.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Model
{
    /// <summary>
    /// 平台通用应答
    /// </summary>
    public class CommonPlatformResp : PlatformResp
    {
        /// <summary>
        /// byte[2-3] 应答ID 对应的终端消息的ID
        /// </summary>
        public short ReplyId { get; set; }

        /// <summary>
        ///  响应码
        ///  0：成功∕确认
        ///  1：失败
        ///  2：消息有误
        ///  3：不支持
        ///  4：报警处理确认
        /// </summary>
        public byte ReplyCode { get; set; }

        public CommonPlatformResp() {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="replyFlowId">应答流水号</param>
        /// <param name="replyId">终端消息的ID</param>
        /// <param name="replyCode">响应码</param>
        public CommonPlatformResp(short replyId, byte replyCode)
        {
            ReplyId = replyId;
            ReplyCode = replyCode;
        }

        public override short GetMessageId()
        {
           return (short) JT808Constant.CMD_COMMON_RESP;
        }
    }
}
