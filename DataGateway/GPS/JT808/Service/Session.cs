using DotNetty.Transport.Channels;
using System;
using System.Net;
using System.Text;

namespace DataGateway.GPS.JT808.Service
{
    public class Session
    {
        /// <summary>
        /// session id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 终端上传的SIM卡号
        /// </summary>
        public string TerminalPhone { get; set; }

        public IChannel Channel { get; set; }

        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsAuthenticated { get; set; }

        // 消息流水号 word(16) 按发送顺序从 0 开始循环累加
        private short _currentFlowId = 0;

        public short CurrentFlowId
        {
            get
            {
                if (_currentFlowId == short.MaxValue)
                    _currentFlowId = 0;

                return _currentFlowId++;
            }
        }

        // private ChannelGroup channelGroup = new
        // DefaultChannelGroup(GlobalEventExecutor.INSTANCE);
        // 客户端上次的连接时间，该值改变的情况:
        // 1. terminal --> server 心跳包
        // 2. terminal --> server 数据包
        public long LastCommunicateTimeStamp { get; set; }


        public static string BuildId(IChannel channel)
        {
            return channel.Id.AsLongText();
        }

        public static Session BuildSession(IChannel channel)
        {
            return BuildSession(channel, null);
        }

        public static Session BuildSession(IChannel channel, string phone)
        {
            Session session = new Session();
            session.Channel = channel;
            session.Id = BuildId(channel);
            session.TerminalPhone = phone;
            
            session.LastCommunicateTimeStamp = DateTime.Now.Millisecond;
            return session;
        }

        public EndPoint getRemoteAddr()
        {
            return Channel.RemoteAddress;
        }

    }
}
