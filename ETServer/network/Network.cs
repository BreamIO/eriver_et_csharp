using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using BinarySerialization;

namespace eriver.network
{
    class StreamReaderWriter
    {
        static BinarySerializer serializer = new BinarySerializer();
        Stream stream;

        public StreamReaderWriter(Stream stream)
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
