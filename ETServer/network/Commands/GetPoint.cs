using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eriver.network.Commands
{
    class GetPoint : Command
    {

        public override ProtocolArgs Deserialize(byte[] v)
        {
            throw new NotImplementedException();
        }

        public override byte[] Serialize(ProtocolArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
