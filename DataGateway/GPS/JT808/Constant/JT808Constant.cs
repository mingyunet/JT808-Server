using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Constant
{
    public class JT808Constant
    {
        public static short MAX_MSG_BODY_LENGTH = 1023;

        public static short MAX_MOBILE_LENGTH = 12;


        public static Encoding STRING_ENCODING = Encoding.GetEncoding("GBK");

        /// <summary>
        /// 标识位
        /// </summary>
        public static byte PKG_DELIMITER = 0x7e;

        /// <summary>
        /// 客户端15分钟后无动作,服务器主动断开连接
        /// </summary>
        public static short TCP_CLIENT_IDLE_MINUTES = 30;

        /// <summary>
        /// 终端通用应答
        /// </summary>
        public static short MSG_ID_TERMINAL_COMMON_RESP = 0x0001;

        /// <summary>
        /// 终端心跳
        /// </summary>
        public static short MSG_ID_TERMINAL_HEART_BEAT = 0x0002;

        /// <summary>
        /// 终端注册
        /// </summary>
        public static short MSG_ID_TERMINAL_REGISTER = 0x0100;

        /// <summary>
        /// 终端注销
        /// </summary>
        public static short MSG_ID_TERMINAL_LOG_OUT = 0x0003;

        /// <summary>
        /// 终端鉴权
        /// </summary>
        public static short MSG_ID_TERMINAL_AUTHENTICATION = 0x0102;


        /// <summary>
        /// 位置信息汇报
        /// </summary>
        public static short MSG_ID_TERMINAL_LOCATION_INFO_UPLOAD = 0x0200;

        /// <summary>
        /// 胎压数据透传
        /// </summary>
        public static short MSG_ID_TERMINAL_TRANSMISSION_TYRE_PRESSURE = 0x0600;

        /// <summary>
        /// 查询终端参数应答
        /// </summary>
        public static short MSG_ID_TERMINAL_PARAM_QUERY_RESP = 0x0104;

        /// <summary>
        /// 平台通用应答
        /// </summary>
        public static int CMD_COMMON_RESP = 0x8001;

        /// <summary>
        /// 终端注册应答
        /// </summary>
        public static int CMD_TERMINAL_REGISTER_RESP = 0x8100;

        /// <summary>
        /// 设置终端参数
        /// </summary>
        public static int CMD_TERMINAL_PARAM_SETTINGS = 0X8103;

        /// <summary>
        /// 查询终端参数
        /// </summary>
        public static int CMD_TERMINAL_PARAM_QUERY = 0x8104;
    }
}
