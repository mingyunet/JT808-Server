using DataGateway.GPS.JT808.Constant;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DataGateway.GPS.JT808.Util
{
    public class DataUtil
    {
        /// <summary>
        /// 从BCD码转日期
        /// </summary>
        /// <param name="bcdYear"></param>
        /// <param name="bcdMonth"></param>
        /// <param name="bcdDay"></param>
        /// <param name="bcdHour"></param>
        /// <param name="bcdMinute"></param>
        /// <param name="bcdSecond"></param>
        /// <returns></returns>
        public static string GetTimeFromBCD(byte bcdYear, byte bcdMonth, byte bcdDay, byte bcdHour, byte bcdMinute,byte bcdSecond)
        {
            int year = 2000 + Bcd2decimal(bcdYear);
            int month = Bcd2decimal(bcdMonth);
            int day = Bcd2decimal(bcdDay);
            int hour = Bcd2decimal(bcdHour);
            int minute = Bcd2decimal(bcdMinute);
            int second = Bcd2decimal(bcdSecond);

            string dateString =  string.Format("{0}-{1}-{2} {3}:{4}:{5}", year,month,day,hour,minute,second);
            //DateTime dt = DateTime.ParseExact(dateString, "yyyy-MM-dd hh:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            return dateString;
        }


        /// <summary>
        /// convert bcd to decimal
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Bcd2decimal(byte b)
        {
            return (b >> 4 & 0x0F) * 10 + (b & 0x0F);
        }

        public static string ByteArrToString(byte[] ByteArr) {
            if (ByteArr == null || ByteArr.Length < 1)
                return "";

            return JT808Constant.STRING_ENCODING.GetString(ByteArr);
        }

        //static Regex reUnicode = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);

        //public static string Decode(string s)
        //{
        //    return reUnicode.Replace(s, m =>
        //    {
        //        short c;
        //        if (short.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out c))
        //        {
        //            return "" + (char)c;
        //        }
        //        return m.Value;
        //    });
        //}
    }
}
