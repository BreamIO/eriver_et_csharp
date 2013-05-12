using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tobii.EyeTracking.IO;

namespace Eriver.Trackers
{
    class TobiiTracker : ITracker
    {

        //private bool running = true;
        private Queue<object> queue;
        //private bool enabled = false;
        //private bool calibrating = false;
        private String etid = null;
        private IEyeTracker et = null;
        

        public TobiiTracker()
        {
            if (!Library.IsInitialized) Library.Init();
            queue = new Queue<object>();
            Thread thread = new Thread(connect);
            thread.Start();
        }

        private void connect() {
            EyeTrackerBrowser browser = new EyeTrackerBrowser();
            browser.EyeTrackerFound += browser_EyeTrackerFound;
            browser.StartBrowsing();
        }

        void browser_EyeTrackerFound(object sender, EyeTrackerInfoEventArgs e)
        {
            EyeTrackerInfo temp_etinfo = e.EyeTrackerInfo;
            if (temp_etinfo.Status == "ok")
                etid = temp_etinfo.ProductId;
                et = e.EyeTrackerInfo.Factory.CreateEyeTracker();
        }

        #region Tracker Members

        void ITracker.RegisterOnETEvent(ETEventHandler callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.Enable(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.Disable(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.GetState(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.StartCalibration(double angle, TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.EndCalibration(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.ClearCalibration(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.AddPoint(double x, double y, TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.GetName(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.GetRates(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.GetRate(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.SetRate(int rate, TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
