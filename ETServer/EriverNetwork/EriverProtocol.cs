using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;
using Eriver.Network;

namespace Eriver.Network
{
    /// <summary>
    /// This Class represents a message in the Eriver protocol
    /// </summary>
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


        /// <summary>
        /// Gives a textual representation of the object.
        /// </summary>
        /// <returns> A string representing the current state of the packet.</returns>
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

        /// <summary>
        /// Determines if this object is by value equal to another.
        /// </summary>
        /// <param name="obj">A object to compare with.</param>
        /// <returns>If the two objects are considered equal, true. False otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false.
            EriverProtocol other = obj as EriverProtocol;
            if ((System.Object)other == null)
            {
                return false;
            }

            if (this.Kind != other.Kind)
            {
                return false;
            }

            switch (Kind)
            {
                case Command.GetPoint: return GetPoint.Equals(other.GetPoint);
                case Command.StartCalibration: return StartCalibration.Equals(other.StartCalibration);
                case Command.EndCalibration: return true;
                case Command.ClearCalibration: return true;
                case Command.AddPoint: return AddPoint.Equals(other.AddPoint);
                case Command.Unavailable: return true;
                case Command.Name: return Name.Equals(other.Name);
                case Command.Fps: return Fps.Equals(other.Fps);
                case Command.KeepAlive: return true;
                default: return false;
            }
            
        }

    }
}
