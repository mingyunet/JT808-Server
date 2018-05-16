using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Model
{
    public class ResponseData
    {
        public string TerminalID { get; set; }
        public long Time { get; set; }
        public int CmdID { get; set; }
        public int CmdSerialNo { get; set; }
        public byte[] MsgBody { get; set; }


        public ResponseData()
        {
        }

        public ResponseData(String terminalID, long time, int cmdID, int cmdSerialNo, byte[] msgBody)
        {
            TerminalID = terminalID;
            Time = time;
            CmdID = cmdID;
            CmdSerialNo = cmdSerialNo;
            MsgBody = msgBody;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public long GetTimeStamp()
        { 
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)ts.TotalSeconds;
        }


    }
}
