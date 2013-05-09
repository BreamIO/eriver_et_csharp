using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eriver.network
{
    public class ProtocolArgs : EventArgs
    {
        private double x, y, angle;
        private long time;
        private int i;
        private string s;

        public double X
        {
            set
            {
                x = value;
            }

            get
            {
                return x;
            }
        }

        public double Y
        {
            set
            {
                y = value;
            }

            get
            {
                return y;
            }
        }

        public double Angle
        {
            set
            {
                angle = value;
            }

            get
            {
                return angle;
            }
        }

        public long Timestamp
        {
            set
            {
                time = value;
            }

            get
            {
                return time;
            }
        }

        public int Integer
        {
            set
            {
                i = value;
            }

            get
            {
                return i;
            }
        }

        public string String
        {
            set
            {
                s = value;
            }

            get
            {
                return s;
            }
        }

    }
}
