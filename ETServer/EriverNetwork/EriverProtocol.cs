using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;
using Eriver.Network;

namespace Eriver.Network
{
    public class EriverProtocol
    {
        public Command Kind { get; set; }

        [SerializeWhen("Kind", Command.GetPoint)]
        public GetPoint GetPoint { get; set; }

        [SerializeWhen("Kind", Command.StartCalibration)]
        public StartCalibration StartCalibration { get; set; }

        [SerializeWhen("Kind", Command.AddPoint)]
        public AddPoint AddPoint { get; set; }

        [SerializeWhen("Kind", Command.Name)]
        public Name Name { get; set; }

        [SerializeWhen("Kind", Command.Fps)]
        public Fps Fps { get; set; }

        public override string ToString()
        {
            switch (Kind)
            {
                case Command.GetPoint: return GetPoint.ToString();
                case Command.StartCalibration: return StartCalibration.ToString();
                case Command.EndCalibration: return "EndCalibration()";
                case Command.ClearCalibration: return "ClearCalibration()";
                case Command.AddPoint: return AddPoint.ToString();
                case Command.Unavailable: return "Unavaliable()";
                case Command.Name: return Name.ToString();
                case Command.Fps: return Fps.ToString();
                case Command.KeepAlive: return "KeepAlive()";
                default: return "Unknown(?)";
            }
        } 

    }
}
