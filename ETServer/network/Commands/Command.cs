using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace eriver.network
{
    public abstract class Command
    {
        private readonly byte id;
        private readonly int length;

        public Command(byte id, int length)
        {
            this.id = id;
            this.length = length;
        }

        public byte Id
        {
            get
            {
                return id;
            }
        }

        public int Length
        {
            get
            {
                return length;
            }
        }

        public abstract ProtocolArgs Deserialize(byte[] v);
        public abstract byte[] Serialize(ProtocolArgs args);
    }
}
