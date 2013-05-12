using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace eriver.network.commands
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

    ///GetPoint is represented as a ETEvent object.

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
