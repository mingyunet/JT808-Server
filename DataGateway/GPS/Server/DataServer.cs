using System;
using System.Threading.Tasks;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging.Console;

namespace DataGateway.gps.server
{
    public class DataServer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 引导辅助程序
        /// </summary>
        private readonly ServerBootstrap _bootstrap;

        /// <summary>
        /// 主工作线程组
        /// </summary>
        private readonly MultithreadEventLoopGroup _bossEventLoopGroup;

        /// <summary>
        /// 工作线程组
        /// </summary>
        private readonly MultithreadEventLoopGroup _workerEventLoopGroup;

        private IChannel _channel;

        /// <summary>
        /// 当服务器请求处理线程全满时，用于临时存放已完成三次握手的请求的队列的最大长度
        /// </summary>
        private int Backlog = 128;

        /// <summary>
        /// 服务端口
        /// </summary>
        private int Port;


        public DataServer(int port)
        {
            this.Port = port;
            _bootstrap = new ServerBootstrap();
            //主工作线程组，设置为1个线程
            _bossEventLoopGroup = new MultithreadEventLoopGroup(1);
            //工作线程组，默认为内核数*2的线程数
            _workerEventLoopGroup = new MultithreadEventLoopGroup();
        }

        public DataServer(int port, int backlog)
        {
            Port = port;
            Backlog = backlog;
            _bootstrap = new ServerBootstrap();
            //主工作线程组，设置为1个线程
            _bossEventLoopGroup = new MultithreadEventLoopGroup(1);
            //工作线程组，默认为内核数*2的线程数
            _workerEventLoopGroup = new MultithreadEventLoopGroup();
        }

        public async Task StartAsync()
        {
            try
            {
                logger.Info("服务器启动");
                Init();
                _channel = await _bootstrap.BindAsync(Port);
                logger.Info("开始监听端口：" + this.Port);
            }
            catch (Exception ex)
            {
                logger.Error("服务器初始化发生异常：" + ex.Message);
            }
        }

        public async Task StopAsync()
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_bossEventLoopGroup != null && _workerEventLoopGroup != null)
            {
                await _bossEventLoopGroup.ShutdownGracefullyAsync();
                await _workerEventLoopGroup.ShutdownGracefullyAsync();
            }
        }

        protected void Init()
        {
            InternalLoggerFactory.DefaultFactory.AddProvider(
            new ConsoleLoggerProvider(
              (text, level) => level >= Microsoft.Extensions.Logging.LogLevel.Warning, true)
              );

            _bootstrap.Group(_bossEventLoopGroup, _workerEventLoopGroup);// 设置主和工作线程组
            _bootstrap.Channel<TcpServerSocketChannel>();// 设置通道模式为TcpSocket

            //当服务器请求处理线程全满时，用于临时存放已完成三次握手的请求的队列的最大长度
            _bootstrap.Option(ChannelOption.SoBacklog, Backlog);//ChannelOption.SO_BACKLOG, 1024
            _bootstrap.Handler(new LoggingHandler(LogLevel.WARN));

            //_bootstrap.ChildHandler(new InboundSessionInitializer(inboundSession));
            //_bootstrap.ChildOption(ChannelOption.SoLinger,0);//Socket关闭时的延迟时间(秒)

            //注册
            _bootstrap.ChildHandler(new DataServerInitializer());

            _bootstrap.ChildOption(ChannelOption.SoKeepalive, true);//是否启用心跳保活机制

            //_bootstrap.ChildOption(ChannelOption.TcpNodelay,true);//启用/禁用Nagle算法
        }
    }
}
