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

    /// <summary>
    /// Object designed to encapsulate all network interactions from the rest of the system.
    /// </summary>
    public class EriverStreamReaderWriter
    {
        static BinarySerializer serializer = new BinarySerializer();
        Stream stream;

        /// <summary>
        /// Primary constructor for the Eriver Stream Reader/Writer.
        /// </summary>
        /// <param name="stream">The stream it should read and write to.</param>
        public EriverStreamReaderWriter(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Reads a message from the associated stream.
        /// </summary>
        /// <returns>The next message found on the stream</returns>
        public EriverProtocol Read() 
        {
                return serializer.Deserialize<EriverProtocol>(stream);
        }

        /// <summary>
        /// Writes a message to the stream
        /// </summary>
        /// <param name="proto">The message to be written</param>
        public void Write(EriverProtocol proto)
        {
            serializer.Serialize(stream, proto);
        }

        
        /// <summary>
        /// Transforms a message into its byte array form.
        /// </summary>
        /// <param name="proto">The message to be transformed</param>
        /// <returns>The resulting array</returns>
        public static byte[] Transform(EriverProtocol proto)
        {
            if (proto == null) return new byte[0];
            int length = CommandConvert.ToLength(proto.Kind) + 1;
            byte[] buf = new byte[length];
            Stream tempStream = new MemoryStream(buf);
            serializer.Serialize(tempStream, proto);
            return buf;
        }

        /// <summary>
        /// Reverse operation.
        /// Transforms a array of bytes to a Eriver message.
        /// </summary>
        /// <param name="buffer">Array of bytes containing a message</param>
        /// <returns>The decoded message</returns>
        public static EriverProtocol Transform(byte[] buffer)
        {
            return serializer.Deserialize<EriverProtocol>(buffer);
        }
    }
}
