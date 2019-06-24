using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DataGateway.GPS.JT808.Megssages;
using DataGateway.GPS.JT808.Codec;
using DataGateway.GPS.JT808.Service;
using DotNetty.Handlers.Timeout;
using DataGateway.GPS.JT808.Model;
using DataGateway.GPS.JT808.Constant;
using DataGateway.GPS.JT808.Messages;

namespace DataGateway.GPS.JT808.Handler
{
    public class JT808ProInHandler : ChannelHandlerAdapter
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private TerminalMsgProcessService msgProcessService;
        private SessionManager sessionManager;

        public JT808ProInHandler()
        {
            sessionManager = SessionManager.Instance;
            msgProcessService = new TerminalMsgProcessService();
        }

        //  重写基类的方法，当消息到达时触发，这里收到消息后，在控制台输出收到的内容，并原样返回了客户端
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            
            try
            {
                var buf = message as IByteBuffer;
                // 读取完整消息到字节数组
                int remaining = buf.ReadableBytes;
                byte[] messageBytes = new byte[remaining];
                buf.GetBytes(0, messageBytes);
                
                // 解析消息字节数组为JT808Message对象
                JT808Message result = JT808ProtoDecoder.Decode(messageBytes);


                // 网关接收数据时间
                //string gatewaytime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //消息正文
                //byte[] cmdconent = result.MsgBody;

                result.channel = context;
                msgProcessService.processMessageData(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "解析数据时出错{0}",ex.Message);
            }
        }


        /// <summary>
        /// 捕获异常，客户端意外断开链接，也会触发
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            logger.Error("发生异常:{0}", exception.Message);
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            Session session = Session.BuildSession(ctx.Channel);
            sessionManager.SessionAdd(session.Id, session);
            logger.Info("终端连接:{0}", ctx.Channel);
        }

        public override void ChannelInactive(IChannelHandlerContext ctx) {
            string sessionId = ctx.Channel.Id.AsLongText();
            Session session = sessionManager.FindBySessionId(sessionId);
            this.sessionManager.SessionRemove(sessionId);
            logger.Info("终端断开连接:{0}", ctx.Channel);
        }
         

    }
}
