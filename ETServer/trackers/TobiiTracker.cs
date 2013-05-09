using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tobii.EyeTracking.IO;

namespace eriver.trackers
{
    class TobiiTracker : Tracker
    {

        private bool running = true;
        private Queue<Something> queue;
        private bool enabled = false;
        private bool calibrating = false;
        private String etid = null;
        private IEyeTracker et = null;
        

        public TobiiTracker()
        {
            if (!Library.IsInitialized) Library.Init();
            queue =  = new Queue();
            Thread thread = new Thread(connect);
            thread.start();
        }

        private void connect() {
            EyeTrackerBrowser browser = new EyeTrackerBrowser();
            browser.EyeTrackerFound += browser_EyeTrackerFound;
            browser.StartBrowsing();
        }

        void browser_EyeTrackerFound(object sender, EyeTrackerInfoEventArgs e)
        {
            EyeTrackerInfo temp_etinfo = e.EyeTrackerInfo
            if (temp_etinfo.Status == "ok")
                etid = temp_etinfo.ProductId;
                et = e.EyeTrackerInfo.Factory.CreateEyeTracker();
        }

        #region Tracker Members

        void Tracker.register_onETEvent(ETEventHandler callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.enable(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.disable(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.getState(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.startCalibration(double angle, TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.endCalibration(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.clearCalibration(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.addPoint(double x, double y, TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.getName(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.getRates(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.getRate(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void Tracker.setRate(int rate, TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
