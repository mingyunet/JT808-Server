using System;
namespace DataGateway.GPS.JT808.Data
{
    /// <summary>
    /// 终端注册信息-消息ID：0x0100 上传
    /// </summary>
    [Serializable]
    public class TerminalRegInfo
    {
        /// <summary>
        /// 省域ID
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 市县域ID
        /// </summary>
        public int CityId{ get; set; }
        
        /// <summary>
        /// 制造商ID,终端制造商编码
        /// </summary>
        public string ManufacturerId{ get; set; }

        /// <summary>
        /// 终端型号 由制造商自行定义
        /// </summary>
        public string TerminalType{ get; set; }

        /// <summary>
        /// 终端ID由大写字母和数字组成-制造商自行定义
        /// </summary>
        public string TerminalId{ get; set; }
        
        /// <summary>
        /// 车牌颜色(BYTE) 车牌颜色，按照 JT/T415-2006 的 5.4.12 未上牌时，取值为0
        /// 0 未上车牌
        /// 1 蓝色
        /// 2 黄色
        /// 3 黑色
        /// 4 白色
        /// 9 其他
        /// </summary>
        public int LicensePlateColor{ get; set; }


        /// <summary>
        /// 车牌 公安交通管理部门颁发的机动车号牌
        /// </summary>
        public string LicensePlate{ get; set; }
    }
}
