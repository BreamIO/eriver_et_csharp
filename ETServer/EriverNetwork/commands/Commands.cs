using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace Eriver.Network
{

    //Suppresed because it must be serialized as a byte.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
    public enum Command : byte
    {
        Unknown = 0,
        GetPoint = 1,
        StartCalibration = 2,
        EndCalibration = 3,
        ClearCalibration = 4,
        AddPoint = 5,
        Unavailable = 6,
        Name = 7,
        Fps = 8,
        KeepAlive = 9
    }

    public static class CommandConvert
    {

        private static Dictionary<Command, int> lengths = new Dictionary<Command, int>
        {
            {Command.GetPoint, 24}, 
            {Command.StartCalibration, 8}, 
            {Command.EndCalibration, 0}, 
            {Command.ClearCalibration, 0}, 
            {Command.AddPoint, 16}, 
            {Command.Unavailable, 0}, 
            {Command.Name, 1}, 
            {Command.Fps, 4}, 
            {Command.KeepAlive, 0}, 
        };

        public static byte ToByte(Command cmd)
        {
            return (byte)cmd;
        }

        public static int ToLength(Command cmd)
        {
            return lengths[cmd];
        }
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
        /// <param name="x_component">Value for X</param>
        /// <param name="yComponent">Value for Y</param>
        /// <param name="time">Timestamp to use for this data point.</param>
        public GetPoint(double xComponent, double yComponent, long time)
        {
            X = xComponent;
            Y = yComponent;
            Timestamp = time;
        }

        /// <summary>
        /// Provides a string representation of the data.
        /// Given on the form Classname(param1; param2; param3;...)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("GetPoint({0:+0.00000;-0.00000}; {1:+0.00000;-0.00000}; {2:D9})", X, Y, Timestamp);
        }
    }

    public class StartCalibration
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public double Angle { get; set; }

        /// <summary>
        /// Provides a string representation of the data.
        /// Given on the form Classname(param1; param2; param3;...)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("StartCalibration({0:+0.00000;-0.00000}", Angle);
        }
    }

    public class AddPoint
    {
        //Suppressed because it should be called X. It is the X component of the coordinate.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
        [SerializeAs(Endianness = Endianness.Big)]
        public double X { get; set; }

        //Suppressed because it should be called Y. It is the Y component of the coordinate.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
        [SerializeAs(Endianness = Endianness.Big)]
        public double Y { get; set; }

        /// <summary>
        /// Provides a string representation of the data.
        /// Given on the form Classname(param1; param2; param3;...)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("AddPoint({0:+0.00000;-0.00000}; {1:+0.00000;-0.00000})", X, Y);
        }
    }

    public class Name
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public byte Value { get; set; }

        /// <summary>
        /// Provides a string representation of the data.
        /// Given on the form Classname(param1; param2; param3;...)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Name({0:D3})", Value);
        }
    }

    public class Fps
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public uint Value { get; set; }

        /// <summary>
        /// Provides a string representation of the data.
        /// Given on the form Classname(param1; param2; param3;...)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Fps({0:D9})", Value);
        }
    }
}
