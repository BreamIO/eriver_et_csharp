using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace Eriver.Network
{

    /// <summary>
    /// Command - Enum
    /// 
    /// Lists all known commands in the Eriver protocol.
    /// Represented as bytes because that is how they are represented in the protocol.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
    public enum Command : byte
    {
        /// <summary>
        /// Not a legal command.
        /// Used to detect if something is wrong in transfer.
        /// If an unknown command is recieved. Connection may be shutdown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// In a provider: Request for start of stream of points.
        /// In a client: A single point of tracking data.
        /// </summary>
        GetPoint = 1,

        /// <summary>
        /// Starts a new calibration session.
        /// </summary>
        StartCalibration = 2,

        /// <summary>
        /// Ends the current calibration routine
        /// </summary>
        EndCalibration = 3,

        /// <summary>
        /// Clears the current calibration routine.
        /// </summary>
        ClearCalibration = 4,
        
        /// <summary>
        /// Used to add a point to the current calibration routine.
        /// </summary>
        AddPoint = 5,
        
        /// <summary>
        /// Error handeling command
        /// If a request is can not be processed at this time, or comes in the wrong order.
        /// A message of this type is sent back.
        /// </summary>
        Unavailable = 6,
        
        /// <summary>
        /// The Name command.
        /// Used to request and respond with the name of a provider.
        /// </summary>
        Name = 7,
        
        /// <summary>
        /// Commands used to request the rate of GetPoint messages comming from a provider.
        /// </summary>
        Fps = 8,
        
        /// <summary>
        /// Unofficial command at the time of writing.
        /// If sent, a simple answer is to be expected.
        /// If no answer is recieved, it is safe to assume 
        /// the other end is not responsive, 
        /// and the connection should be terminated
        /// </summary>
        KeepAlive = 9
    }

    public static class CommandConvert
    {

        private static Dictionary<Command, int> lengths = new Dictionary<Command, int>
        {
            {Command.Unknown, 0},
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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false.
            GetPoint other = obj as GetPoint;
            if ((System.Object)other == null)
            {
                return false;
            }

            return (Math.Abs(this.X - other.X) < 0.01) &&
                (Math.Abs(this.Y - other.Y) < 0.01) &&
                (this.Timestamp == other.Timestamp);
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
            return String.Format("StartCalibration({0:+0.00000})", Angle);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false.
            StartCalibration other = obj as StartCalibration;
            if ((System.Object)other == null)
            {
                return false;
            }

            return (Math.Abs(this.Angle - other.Angle) < 0.01);
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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false.
            AddPoint other = obj as AddPoint;
            if ((System.Object)other == null)
            {
                return false;
            }

            return (Math.Abs(this.X - other.X) < 0.01) && (Math.Abs(this.Y - other.Y) < 0.01);
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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false.
            Name other = obj as Name;
            if ((System.Object)other == null)
            {
                return false;
            }

            return (this.Value == other.Value);
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
            return String.Format("Fps({0})", Value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false.
            Fps other = obj as Fps;
            if ((System.Object)other == null)
            {
                return false;
            }

            return (this.Value == other.Value);
        }
    }
}
