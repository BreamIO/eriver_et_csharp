using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace eriver
{
    public class ETEvent
    {
        [SerializeAs(Endianness = Endianness.Big)]
        public double X { get; set; }
        public double Y { get; set; }

        [SerializeAs(Endianness = Endianness.Big)]
        public long Timestamp { get; set; }

        public ETEvent(double x, double y, long timestamp)
        {
            this.X = x;
            this.Y = y;
            this.Timestamp = timestamp;
        }

        public override string ToString()
        {
            return String.Format("ETEvent({0}; {1}; {2})", X, Y, Timestamp);
        }
    }
}
