using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;
using Eriver.Network.Commands;

using Eriver.Network;

namespace Eriver.Network
{
    class EriverProtocol
    {
        public Command Type { get; set; }

        [SerializeWhen("Type", Command.GetPoint)]
        public GetPoint GetPoint { get; set; }

        [SerializeWhen("Type", Command.StartCalibration)]
        public StartCalibration StartCalibration { get; set; }

        [SerializeWhen("Type", Command.AddPoint)]
        public AddPoint AddPoint { get; set; }

        [SerializeWhen("Type", Command.Name)]
        public Name Name { get; set; }

        [SerializeWhen("Type", Command.Fps)]
        public Fps Fps { get; set; }

    }
}
