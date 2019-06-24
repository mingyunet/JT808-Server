using DataGateway.GPS.JT808.Constant;
using JT808DataServer.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataGateway.GPS.JT808.Messages
{
    /// <summary>
    /// JT808消息体属性
    /// </summary>
    public class JT808MessageBodyAttr
    {
        /// <summary>
        /// 消息体长度
        /// 10 bits
        /// </summary>
        public int MsgBodyLength { get; set; }

        /// <summary>
        /// 数据加密方式 NON(0), RSA(1);
        /// </summary>
        public int EncryptionType { get; set; }

        /// <summary>
        /// 是否分包
        /// Not a complete package, it's split. 1 bit
        /// </summary>
        public bool IsSplit { get; set; }

        /// <summary>
        /// 保留数据 
        /// 2 bits
        /// </summary>
        public int Reserve { get; set; }

        /// <summary>
        /// 编码为字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] Encode()
        {

            int init = 0x0;

            // is package split
            if (IsSplit)
            {
                init = (init | 0x1) << 13;
            }

            // encrypt type
            init = init | (EncryptionType << 10);

            // message body length
            if (MsgBodyLength> JT808Constant.MAX_MSG_BODY_LENGTH)
            {
                throw new Exception("Field: msgBodyLength="
                        + MsgBodyLength + ", but max allowable values is "
                        + JT808Constant.MAX_MSG_BODY_LENGTH + ".");
            }

            init = init | MsgBodyLength;

            ByteBuffer buffer = ByteBuffer.Allocate(2);
            buffer.PutShort((short)init);

            return buffer.Array();
        }

    }
     
}
