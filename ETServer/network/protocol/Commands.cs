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

    class GetPoint
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public double X { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public double Y { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public long Timestamp { get; set; }
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
        public byte Name { get; set; }
    }

    class Fps
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public uint Fps { get; set; }
    }
}
