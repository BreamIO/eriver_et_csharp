using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace Eriver.Network.Commands
{

    enum Command : byte
    {
        GetPoint = 1,
        StartCalibration = 2,
        EndCalibration = 3,
        ClearCalibraton = 4,
        AddPoint = 5,
        Unavaliable = 6,
        Name = 7,
        Fps = 8,
        KeepAlive = 9
    }

    /// <summary>
    /// Class representing the data of a GetPoint package.
    /// </summary>
    public class GetPoint
    {
        /// <summary>
        /// Used to hold the X part of the coordinate.
        /// Represented in Big (Network) endian while in transit.
        /// </summary>
        [SerializeAs(Endianness = Endianness.Big)]
        public double X { get; set; }

        /// <summary>
        /// Used to hold the Y part of the coordinate.
        /// Represented in Big (Network) endian while in transit.
        /// </summary>
        [SerializeAs(Endianness = Endianness.Big)]
        public double Y { get; set; }

        /// <summary>
        /// Used to hold the timestamp for the creation of the data.
        /// Represented in Big (Network) endian while in transit.
        /// </summary>
        [SerializeAs(Endianness = Endianness.Big)]
        public long Timestamp { get; set; }

        /// <summary>
        /// Default constructor.
        /// Sets all fields to respective zero-value.
        /// </summary>
        public GetPoint()
        {
            X = 0;
            Y = 0;
            Timestamp = 0;
        }

        /// <summary>
        /// Creation constructor
        /// Allows you to specify all values at creation time.
        /// </summary>
        /// <param name="x">Value for X</param>
        /// <param name="y">Value for Y</param>
        /// <param name="time">Timestamp to use for this data point.</param>
        public GetPoint(double x, double y, long time)
        {
            X = x;
            Y = y;
            Timestamp = time;
        }

        /// <summary>
        /// Provides a string representation of the data. 
        /// Given on the form Classname(param1; param2; param3)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("ETEvent({0:+0.00000;-0.00000}; {1:+0.00000;-0.00000}; {2:D9})", X, Y, Timestamp);
        }
    }

    class StartCalibration
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public double Angle { get; set; }
    }

    class AddPoint
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public double X { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public double Y { get; set; }
    }

    class Name
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public byte Value { get; set; }
    }

    class Fps
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public uint Value { get; set; }
    }
}
