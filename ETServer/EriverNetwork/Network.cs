using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using BinarySerialization;
using Eriver.Network;

namespace Eriver.Network
{
    public class EriverStreamReaderWriter
    {
        static BinarySerializer serializer = new BinarySerializer();
        Stream stream;

        public EriverStreamReaderWriter(Stream stream)
        {
            this.stream = stream;
        }

        public EriverProtocol Read() 
        {
                return serializer.Deserialize<EriverProtocol>(stream);
        }

        public void Write(EriverProtocol proto)
        {
            serializer.Serialize(stream, proto);
        }

        

        public static byte[] Transform(EriverProtocol proto)
        {
            if (proto == null) return new byte[0];
            int length = CommandConvert.ToLength(proto.Kind) + 1;
            byte[] buf = new byte[length];
            Stream tempStream = new MemoryStream(buf);
            serializer.Serialize(tempStream, proto);
            return buf;
        }

        public static EriverProtocol Transform(byte[] buffer)
        {
            return serializer.Deserialize<EriverProtocol>(buffer);
        }
    }
}
