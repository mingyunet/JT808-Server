using System;
using System.Collections.Generic;
using System.Text;

namespace JT808DataServer.Common
{
    public class DateTimeEx
    {
        /// <summary>
        /// 获取系统时间戳-毫秒
        /// </summary>
        public static long CurrentTimeMillis
        {
            get
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt64(ts.TotalSeconds * 1000);
            }
        }
    }
}
