using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eriver
{
    class Protocol
    {
        public delegate void GetPointDelegate(double x, double y, long time);
        public delegate void StartCalDelegate(double angle);
        public delegate void EndCalDelegate();
        public delegate void ClearCalDelegate();
        public delegate void AddPointDelegate(double x, double y);
        public delegate void NameDelegate(int name);
        public delegate void FpsDelegate(int fps);
        public delegate void CheeseDelegate(String cheese);
        public delegate void TeapotDelegate(String answer);

        private GetPointDelegate getpoint;
        private StartCalDelegate startcal;
        private EndCalDelegate endcal;

        #region setDelegates
        public void setDelegate(GetPointDelegate del)
        {
            getpoint = del;
        }

        public void setDelegate(StartCalDelegate del)
        {
            startcal = del;
        }

        public void setDelegate(EndCalDelegate del)
        {
            endcal = del;
        }

        #endregion

    }
}
