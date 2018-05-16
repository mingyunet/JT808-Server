using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DataGateway.GPS.JT808.Handler;

namespace DataGateway.gps.server
{
    public class DataServerInitializer : ChannelInitializer<ISocketChannel>
    {
        protected override void InitChannel(ISocketChannel channel)
        {
            var pipeline = channel.Pipeline;

            
            pipeline.AddLast("DebugLogging",
                new LoggingHandler(LogLevel.INFO));

            pipeline.AddLast("inhandler", new JT808ProInHandler());
        }
    }
}
