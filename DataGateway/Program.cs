using DataGateway.gps.server;
using DataGateway.GPS.JT808.Util;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace DataGateway
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        { 
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //注册GBK编码后面的解析 Btye[] 转String需要用到
            //Console.WriteLine(Encoding.GetEncoding("GBK"));
            DataServer jt808server = new DataServer(9623);
            jt808server.StartAsync().Wait();

            Console.ReadLine();
        }
    }
}
