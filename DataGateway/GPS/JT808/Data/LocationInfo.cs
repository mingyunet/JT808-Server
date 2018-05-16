using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Data
{
    /// <summary>
    /// 位置信息
    /// </summary>
    [Serializable]
    public class LocationInfo
    {
        /// <summary>
        /// 报警标志
        /// </summary>
        public AlarmInfo AlarmState { get; set; }

        /// <summary>
        /// 状态信息
        /// </summary>
        public WorkStateInfo StateInfo { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public Double Lon { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public Double Lat { get; set; }

        /// <summary>
        /// 偏移经度
        /// </summary>
        public Double ShiftLon { get; set; }

        /// <summary>
        /// 偏移纬度
        /// </summary>
        public Double ShiftLat { get; set; }

        /// <summary>
        /// 海拔高度，单位为米（m）
        /// </summary>
        public Double Altitude { get; set; }

        /// <summary>
        /// 速度  km/h
        /// </summary>
        public Double Speed { get; set; }

        /// <summary>
        /// 方向 0-359，正北为 0，顺时针
        /// </summary>
        public int Direction { get; set; }

        /// <summary>
        /// gps时间
        /// </summary>
        public string GpsTime { get; set; }
    }

    /// <summary>
    /// 报警状态位
    /// </summary>
    public class AlarmInfo
    {
        /// <summary>
        /// 超速报警
        /// </summary>
        public bool OverSpeed { get; set; }

        /// <summary>
        /// GNSS 模块发生故障
        /// </summary>
        public bool GnssFault { get; set; }


        /// <summary>
        /// 终端主电源欠压
        /// </summary>
        public bool PowerLow { get; set; }

        /// <summary>
        /// 终端主电源掉电
        /// </summary>
        public bool PowerOff { get; set; }

    }

    /// <summary>
    /// 状态位
    /// </summary>
    public class WorkStateInfo
    {
        /// <summary>
        /// ACC状态
        /// </summary>
        public bool AccOn { get; set; }

        /// <summary>
        /// 导航状态
        /// </summary>
        public bool Navigation { get; set; }

        /// <summary>
        /// 0：北纬；1：南纬
        /// </summary>
        public int Latitude_N_S { get; set; }

        /// <summary>
        /// 0：东经；1：西经
        /// </summary>
        public int Longitude_E_W { get; set; }

    }

}
