using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eriver.trackers
{
    class MockTracker : Tracker
    {

        List<ETEventHandler> onETEvent; //Should be List, but doesn't work for some reason...
        bool active;
        bool calibrating;
        byte name;
        int fps;
        double step;


        public MockTracker(byte name, int fps)
        {
            this.active=false;
            this.calibrating=false;
            this.name=name;
            this.fps = fps;
            step = 0.01;

            onETEvent = new List<ETEventHandler>();

            Thread thread = new Thread(new ThreadStart(this.run));
            thread.Start();
        }

        public void run()
        {
            foreach (ETEvent e in generateCircle())
            {
                foreach (ETEventHandler c in onETEvent)
                {
                    c(e);
                }
            }
        }

        #region Generators

        private IEnumerable<ETEvent> generateCircle()
        {
            double t = 0;
            while(true) 
            {
                yield return new ETEvent(Math.Cos(t), Math.Sin(t), (long)(DateTime.UtcNow-new DateTime (1970, 1, 1)).TotalMilliseconds);
                t+=step;
                Thread.Sleep(1000 / fps);
            }
        }

        #endregion

        #region Tracker Members

        void Tracker.register_onETEvent(ETEventHandler callback)
        {
            onETEvent.Add(callback);
        }

        void Tracker.enable(TrackerCallback callback)
        {
            active = true;
            callback(0, 1);
        }

        void Tracker.disable(TrackerCallback callback)
        {
            active = false;
            callback(0, 1);
        }

        void Tracker.getState(TrackerCallback callback)
        {
            int status = 0 + (active ? 1:0) + (calibrating ? 1:0 << 1);
            callback(0, status);
        }

        void Tracker.startCalibration(double angle, TrackerCallback callback)
        {
            calibrating = true;
            callback(0, 1);
        }

        void Tracker.endCalibration(TrackerCallback callback)
        {
            calibrating = false;
            callback(0, 1);
        }

        void Tracker.clearCalibration(TrackerCallback callback)
        {
            callback(calibrating ? 0: 1, 1);
        }

        void Tracker.addPoint(double x, double y, TrackerCallback callback)
        {
            callback(calibrating ? 0 : 1, 1);
        }

        void Tracker.getName(TrackerCallback callback)
        {
            callback(0, name);
        }

        void Tracker.getRates(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.getRate(TrackerCallback callback)
        {
            callback(0, fps);
        }

        void Tracker.setRate(int rate, TrackerCallback callback)
        {
            fps = rate;
            callback(0, fps);
        }

        #endregion
    }
}
