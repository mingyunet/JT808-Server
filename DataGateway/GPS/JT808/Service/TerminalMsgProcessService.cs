using System;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using DataGateway.GPS.JT808.Constant;
using DataGateway.GPS.JT808.Megssages;
using DataGateway.GPS.JT808.Messages;
using DataGateway.GPS.JT808.Model;
using DataGateway.GPS.JT808.Data;
using DataGateway.GPS.JT808.Util;
using DotNetty.Buffers;
using System.Text;
using JT808DataServer.Common;

namespace DataGateway.GPS.JT808.Service
{
    public class TerminalMsgProcessService : BaseMsgProcessService
    {

        public TerminalMsgProcessService()
        {
        }


        public void processMessageData(JT808Message req)
        {
            JT808MessageHead header = req.MsgHeader;
            // 1. 终端心跳-消息体为空 ==> 平台通用应答
            if (header.MessageId == JT808Constant.MSG_ID_TERMINAL_HEART_BEAT)
            {
                logger.Info("<<<<<[终端心跳],phone={0},flowid={1}", header.TerminalId, header.MessageSerial);
                ProcessCommonResponse(req);
            }//2. 终端注册 ==> 终端注册应答
            else if (header.MessageId == JT808Constant.MSG_ID_TERMINAL_REGISTER)
            {
                ProcessTerminalRegist(req);
                logger.Info(">>>>>[终端注册],终端ID={0},流水号={1}", header.TerminalId, header.MessageSerial);

            }//3.终端鉴权 ==> 平台通用应答
            else if (header.MessageId == JT808Constant.MSG_ID_TERMINAL_AUTHENTICATION)
            {
                logger.Info(">>>>>[终端鉴权],终端ID={0},流水号={1}", header.TerminalId, header.MessageSerial);
                ProcessCommonResponse(req);
            }//4.终端注销(终端注销数据消息体为空) ==> 平台通用应答
            else if (header.MessageId == JT808Constant.MSG_ID_TERMINAL_LOG_OUT)
            {
                logger.Info(">>>>>[终端注销],终端ID={0},流水号={1}", header.TerminalId, header.MessageSerial);

            }//5.位置信息汇报 ==> 平台通用应答
            else if (header.MessageId == JT808Constant.MSG_ID_TERMINAL_LOCATION_INFO_UPLOAD)
            {
                ProcessLocationInfo(req);
                logger.Info(">>>>>[位置信息汇报],终端ID={0},流水号={1}", header.TerminalId, header.MessageSerial);

            }// 其他情况
            else
            {
                logger.Info(">>>>>[其他情况],终端ID={0},流水号={1},消息ID={2}", header.TerminalId, header.MessageSerial, header.MessageId);

            }
        }

        #region 协议解析

        /// <summary>
        /// 终端注册
        /// </summary>
        /// <param name="req"></param>
        private void ProcessTerminalRegist(JT808Message req)
        {
            try
            {
                #region 终端注册解析
                byte[] data = req.MsgBody;
                ByteBuffer buf = ByteBuffer.Wrap(data);
                TerminalRegInfo reginfo = new TerminalRegInfo();
                // 1. byte[0-1] 省域ID(WORD)
                reginfo.ProvinceId = buf.GetShort();

                // 2. byte[2-3] 设备安装车辆所在的市域或县域,市县域ID采用GB/T2260中规定的行 政区划代码6位中后四位
                reginfo.CityId = buf.GetShort();

                // 3. byte[4-8] 制造商ID(BYTE[5]) 5 个字节，终端制造商编码
                byte[] ProductByte = new byte[5];
                for (int i = 0; i < ProductByte.Length; i++)
                {
                    ProductByte[i] = buf.Get();
                }
                reginfo.ManufacturerId = DataUtil.ByteArrToString(ProductByte);


                // 4. byte[9-16] 终端型号(BYTE[8]) 八个字节， 此终端型号 由制造商自行定义 位数不足八位的，补空格
                byte[] ProductTypeByte = new byte[8];
                for (int i = 0; i < ProductTypeByte.Length; i++)
                {
                    ProductTypeByte[i] = buf.Get();
                }
                reginfo.TerminalType = DataUtil.ByteArrToString(ProductTypeByte);


                // 5. byte[17-23] 终端ID(BYTE[7]) 七个字节， 由大写字母 和数字组成， 此终端 ID由制造 商自行定义
                byte[] TeridByte = new byte[7];
                for (int i = 0; i < TeridByte.Length; i++)
                {
                    TeridByte[i] = buf.Get();
                }
                reginfo.TerminalId = DataUtil.ByteArrToString(TeridByte);


                // 6. byte[24] 车牌颜色(BYTE) 车牌颜 色按照JT/T415-2006 中5.4.12 的规定
                reginfo.LicensePlateColor = buf.Get();

                // 7. byte[25-x] 车牌(STRING) 公安交 通管理部门颁 发的机动车号牌
                byte[] LicenseByte = new byte[data.Length - 25];
                for (int i = 0; i < LicenseByte.Length; i++)
                {
                    LicenseByte[i] = buf.Get();
                }
                reginfo.LicensePlate = DataUtil.ByteArrToString(LicenseByte);
                #endregion
                logger.Info("收到终端注册信息:{0}", JsonConvert.SerializeObject(reginfo));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "终端注册0x0100解析异常:{0}", ex.Message);
            }
            ProcessRegistResponse(req);

        }

        private void ProcessLocationInfo(JT808Message req)
        {
            try
            {
                #region 位置数据解析
                byte[] bytes = req.MsgBody;
                ByteBuffer buf = ByteBuffer.Wrap(bytes);

                // 报警标识 byte[0-3]
                int alarm = buf.GetInt();
                AlarmInfo alarminfo = new AlarmInfo();
                alarminfo.OverSpeed = ((alarm >> 1) & 0x1) == 1;
                alarminfo.GnssFault = ((alarm >> 4) & 0x1) == 1;
                alarminfo.PowerLow = ((alarm >> 7) & 0x1) == 1;
                alarminfo.PowerOff = ((alarm >> 8) & 0x1) == 1;

                //byte[4-7] 状态(DWORD(32))
                int state = buf.GetInt();
                WorkStateInfo stateinfo = new WorkStateInfo();
                stateinfo.AccOn = (state & 0x1) == 1;//0：ACC 关；1： ACC 开
                stateinfo.Navigation = (state >> 1 & 0x1) == 1;// 0：未定位；1：定位
                stateinfo.Latitude_N_S = (state >> 2);
                stateinfo.Longitude_E_W = (state >> 3);

                LocationInfo locationInfo = new LocationInfo();
                locationInfo.AlarmState = alarminfo;
                locationInfo.StateInfo = stateinfo;

                // byte[8-11] 纬度(DWORD(32))
                //以度为单位的纬度值乘以 10 的 6 次方，精确到百万分之一度
                locationInfo.Lat = buf.GetInt() / 1000000.0;

                // byte[12-15] 经度(DWORD(32))
                //以度为单位的经度值乘以 10 的 6 次方，精确到百万分之一度
                locationInfo.Lon = buf.GetInt() / 1000000.0;

                //locationInfo.ShiftLat
                //locationInfo.ShiftLon

                //byte[16-17] 高程(WORD(16)) 海拔高度，单位为米（ m）
                double altitude = buf.GetShort();
                locationInfo.Altitude = altitude;

                // byte[18-19] 速度(WORD) 1/10km/h
                double speed = buf.GetShort() / 10.0;
                locationInfo.Speed = speed;

                // byte[20-21] 方向(WORD) 0-359，正北为 0，顺时针
                int direction = buf.GetShort();
                locationInfo.Direction = direction;

                // byte[22-x] 时间(BCD[6]) YY-MM-DD-hh-mm-ss
                // GMT+8 时间，本标准中之后涉及的时间均采用此时区
                locationInfo.GpsTime = DataUtil.GetTimeFromBCD(buf.Get(), buf.Get(), buf.Get(), buf.Get(), buf.Get(), buf.Get());
                #endregion
                logger.Info("收到位置信息:{0}", JsonConvert.SerializeObject(locationInfo));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "位置信息0x0200解析异常:{0}", ex.Message);
            }
            ProcessCommonResponse(req);
        }
        #endregion

        #region 应答相关
        /// <summary>
        /// 终端注册应答
        /// </summary>
        private void ProcessRegistResponse(JT808Message req)
        {
            try
            {
                #region 终端注册应答
                // Terminal register
                TerminalRegPlatformResp resp = new TerminalRegPlatformResp();
                // 手机号作为鉴权码
                resp.AuthCode = req.MsgHeader.TerminalId;
                // 应答流水号
                resp.ReplySerial = req.MsgHeader.MessageSerial;
                // 终端ID 
                resp.TerminalId = req.MsgHeader.TerminalId;
                // 应答结果
                resp.ReplyCode = MessageResult.Success;
                DoResponse(req.channel, resp);
                #endregion
            }
            catch (Exception e)
            {
                logger.Error(e, "<<<<<[终端注册应答信息]处理错误>>>>>,{0}", e.Message);
            }
        }
        /// <summary>
        /// 平台通用应答
        /// </summary>
        /// <param name="req"></param>
        private void ProcessCommonResponse(JT808Message req)
        {
            try
            {
                //logger.Debug("平台通用应答信息:{0}", JsonConvert.SerializeObject(req));
                JT808MessageHead reqHeader = req.MsgHeader;
                CommonPlatformResp respMsgBody = new CommonPlatformResp();
                CommonPlatformResp resp = new CommonPlatformResp();
                resp.TerminalId = req.MsgHeader.TerminalId;
                resp.ReplySerial = req.MsgHeader.MessageSerial;
                resp.ReplyId = req.MsgHeader.MessageId;
                resp.ReplyCode = MessageResult.Success;
                DoResponse(req.channel, resp);
            }
            catch (Exception e)
            {
                logger.Error(e, "<<<<<[平台通用应答信息]处理错误>>>>>,{0}", e.Message);
            }
        }

        /// <summary>
        /// 向客户端发送应答
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resp"></param>
        private void DoResponse(IChannelHandlerContext context, PlatformResp resp)
        {
            // 构建消息体属性对象
            JT808MessageBodyAttr attr = new JT808MessageBodyAttr();
            attr.EncryptionType = 0;
            byte[] body = resp.Encode();
            attr.MsgBodyLength = body.Length;
            attr.IsSplit = false;

            JT808MessageHead head = new JT808MessageHead();
            head.MessageId = resp.GetMessageId();
            head.MessageSerial = GetFlowId(context.Channel);
            head.TerminalId = resp.TerminalId;
            head.MsgBodyAttr = attr;

            // 构建JT/T808协议消息对象
            JT808Message message = new JT808Message();
            message.MsgHeader = head;
            message.MsgBody = body;

            byte[] reply = message.Encode();
            context.WriteAndFlushAsync(Unpooled.CopiedBuffer(reply));


            //ResponseData response = new ResponseData();
            //response.CmdID = message.MsgHeader.MessageId;
            //response.CmdSerialNo = message.MsgHeader.MessageSerial;
            //response.MsgBody = reply;
            //response.TerminalID = resp.TerminalId;
            //response.Time = response.GetTimeStamp();
            //context.WriteAndFlushAsync(response);


        }
        #endregion
    }


}
