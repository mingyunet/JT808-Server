using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Service
{
    public sealed class SessionManager
    {
        /// <summary>
        /// Netty生成的sessionID和Session的对应关系
        /// key = seession id
        /// value = Session
        /// </summary>
        private ConcurrentDictionary<string, Session> SessionIdMap = new ConcurrentDictionary<string, Session>();

        /// <summary>
        /// 终端手机号和netty生成的sessionID的对应关系
        /// key = 手机号
        /// value = seession id
        /// </summary>
        private ConcurrentDictionary<string, string> PhoneMap = new ConcurrentDictionary<string, string>();

        private static volatile SessionManager instance = null;
        private static readonly object syncRoot = new object();
        private static readonly object InLock = new object();
        private static readonly object OutLock = new object();
        private static readonly object FindLock = new object();



        private SessionManager() { }

        public static SessionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SessionManager();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 会话注册
        /// </summary>
        /// <param name="sessionid"></param>
        /// <param name="session"></param>
        public void SessionAdd(string sessionid, Session session)
        {
            lock (InLock)
            {
                try
                {

                    if (session.TerminalPhone != null && session.TerminalPhone.Trim().Length > 0)
                    {
                        PhoneMap.AddOrUpdate(session.TerminalPhone, session.Id, (key, value) => { return value = session.Id; });
                    }
                    SessionIdMap.AddOrUpdate(sessionid, session, (key, value) => { return value = session; });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Session Add Exception," + ex.Message);
                }
            }
        }

        /// <summary>
        /// 会话查找
        /// </summary>
        /// <param name="sessionid"></param>
        /// <returns></returns>
        public Session FindBySessionId(string sessionid)
        {
            lock (FindLock)
            {
                Session session = null;
                try
                {
                    SessionIdMap.TryGetValue(sessionid, out session);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Session Find Exception," + ex.Message);
                }
                return session;
            }
        }

        /// <summary>
        /// 会话移除
        /// </summary>
        /// <param name="sessionid"></param>
        /// <param name="session"></param>
        public void SessionRemove(string sessionId)
        {
            lock (OutLock)
            {
                if (sessionId == null)
                    return;

                try
                {
                    Session session = null;
                    SessionIdMap.TryRemove(sessionId, out session);

                    if (session == null)
                        return;

                    string sessionid;
                    if (session.TerminalPhone != null)
                        PhoneMap.TryRemove(session.TerminalPhone, out sessionid);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Session Remove Exception," + ex.Message);
                }
            }
        }
    }
}
