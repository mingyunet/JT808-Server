using JT808DataServer.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Messages
{
    public class MessageSplitInfo
    {
        /// <summary>
        /// 消息总包数
        /// </summary>
        public short Packages { get; set; }

        /// <summary>
        /// 包序号，从1开始
        /// </summary>
        public short PackageNo { get; set; }

        /// <summary>
        /// 编码为字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] Encode()
        {
            ByteBuffer buffer = ByteBuffer.Allocate(4);
            buffer.PutShort(Packages);
            buffer.PutShort(PackageNo);
            return buffer.Array();
        }
    }
}
