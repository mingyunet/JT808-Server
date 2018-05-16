using DataGateway.gps.server;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Service
{
    public class BaseMsgProcessService
    {
        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected SessionManager sessionManager;

        public BaseMsgProcessService()
        {
            sessionManager = SessionManager.Instance;
        }

        /// <summary>
        /// 获取应答流水号
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected short GetFlowId(IChannel channel, short defaultValue)
        {
            Session session = sessionManager.FindBySessionId(Session.BuildId(channel));
            if (session == null)
            {
                return defaultValue;
            }

            return session.CurrentFlowId;
        }

        /// <summary>
        /// 获取应答流水号
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        protected short GetFlowId(IChannel channel)
        {
            return GetFlowId(channel, 0);
        }

    }
}
