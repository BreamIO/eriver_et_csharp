using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eriver
{
    public class ETEvent
    {
        double x, y;
        long timestamp;

        public ETEvent(double x, double y, long timestamp)
        {
            this.x = x;
            this.y = y;
            this.timestamp = timestamp;
        }

        public override string ToString()
        {
            return String.Format("ETEvent({0}, {1}, {2})", x, y, timestamp);
        }
    }
}
