using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using BinarySerialization;

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
    }
}
