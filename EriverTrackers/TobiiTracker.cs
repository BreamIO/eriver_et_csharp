using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tobii.EyeTracking.IO;
using Eriver.Network;

namespace Eriver.Trackers
{
    class TobiiTracker : ITracker
    {

        public static readonly int BEST_VALIDITY = 0;

        private Queue<object> queue;
        private bool enabled;
        private bool calibrating;
        private String etid = null;
        private byte name;
        private IEyeTracker et = null;
        public event ETEventHandler EtEventHandlers;
        private Point2D eye_distance;
        

        public TobiiTracker(byte name)
        {
            if (!Library.IsInitialized) Library.Init();
            this.name = name;
            enabled = false;
            calibrating = false;
            eye_distance = new Point2D(5, 0); /*Bad initial approximation*/
            queue = new Queue<object>();
            Thread thread = new Thread(connect);
            thread.Start();
        }

        private void connect() {
            EyeTrackerBrowser browser = new EyeTrackerBrowser();
            browser.EyeTrackerFound += browser_EyeTrackerFound;
            browser.StartBrowsing();
        }

        #region Event Handlers
        void browser_EyeTrackerFound(object sender, EyeTrackerInfoEventArgs e)
        {
            EyeTrackerInfo temp_etinfo = e.EyeTrackerInfo;
            if (temp_etinfo.Status == "ok")
            {
                etid = temp_etinfo.ProductId;
                et = e.EyeTrackerInfo.Factory.CreateEyeTracker();

                //Setup event handlers for this tracker.
                et.CalibrationStarted += et_CalibrationStarted;
                et.CalibrationStopped += et_CalibrationStopped;
                et.ConnectionError += et_ConnectionError;
                et.GazeDataReceived += et_GazeDataRecieved;
            }
        }

        private void et_GazeDataRecieved(object sender, GazeDataEventArgs e)
        {
            if (e.GazeDataItem.RightValidity != BEST_VALIDITY && e.GazeDataItem.LeftValidity != BEST_VALIDITY)
            {
                return;
            }

            GetPoint point = Henshin(e.GazeDataItem);
            point.Timestamp = e.GazeDataItem.Timestamp;

            if (EtEventHandlers != null)
                EtEventHandlers(point);
        }

        void et_ConnectionError(object sender, ConnectionErrorEventArgs e)
        {
            et.Dispose();
            enabled = false;
            et = null;
        }

        void et_CalibrationStopped(object sender, CalibrationStoppedEventArgs e)
        {
            calibrating = false;
        }

        void et_CalibrationStarted(object sender, CalibrationStartedEventArgs e)
        {
            calibrating = true;
        }

        #endregion


        private GetPoint Henshin(IGazeDataItem gazeDataItem)
        {
            int leftValidity  = gazeDataItem.LeftValidity;
            int rightValidity = gazeDataItem.RightValidity;
            Point2D left      = gazeDataItem.LeftGazePoint2D;
            Point2D right     = gazeDataItem.RightGazePoint2D;
            GetPoint answer = new GetPoint();

            if (leftValidity == BEST_VALIDITY && rightValidity == BEST_VALIDITY)
            {
                eye_distance = new Point2D(right.X - left.X, right.Y - left.Y);
            } else if (leftValidity == BEST_VALIDITY)
            {
                right.X = left.X + eye_distance.X;
                right.X = left.Y + eye_distance.Y;
            } else if (rightValidity == BEST_VALIDITY) {
                right.X = left.X - eye_distance.X;
                right.X = left.Y - eye_distance.Y;
            } else {

            }

            answer.X = (left.X + right.X) / 2;
            answer.Y = (left.Y + right.Y) / 2;
            return answer;
        }

        #region Tracker Members

        void ITracker.RegisterOnETEvent(ETEventHandler callback)
        {
            EtEventHandlers += callback;
        }

        

        void ITracker.Enable(TrackerCallback callback)
        {
            et.StartTracking();
            enabled = true;
            callback(0, 1);
            throw new NotImplementedException();
        }

        void ITracker.Disable(TrackerCallback callback)
        {
            et.StopTracking();
            enabled = false;
            callback(0, 1);
            throw new NotImplementedException();
        }

        void ITracker.GetState(TrackerCallback callback)
        {
            int status = 0;
            status |= enabled ? 1: 0;
            status |= calibrating ? 1<<2 : 0;
            callback(0, status);
        }

        void ITracker.StartCalibration(double angle, TrackerCallback callback)
        {
            if (et == null)
            {
                callback(1, 0);
            }
            et.StartCalibration();
            calibrating = true;
            callback(0, 1);
        }

        void ITracker.EndCalibration(TrackerCallback callback)
        {
            if (et == null)
            {
                callback(1, 0);
            }
            et.ComputeCalibration();
            et.StopCalibration();
            calibrating = true;
            callback(0, 1);
        }

        void ITracker.ClearCalibration(TrackerCallback callback)
        {
            if (et == null)
            {
                callback(1, 0);
            }
            et.ClearCalibration();
            callback(0, 1);
        }

        void ITracker.AddPoint(double x, double y, TrackerCallback callback)
        {
            if (et == null)
            {
                callback(1, 0);
            }
            et.AddCalibrationPoint(new Point2D(x, y));
            callback(0, 1);
        }

        void ITracker.GetName(TrackerCallback callback)
        {
            callback(0, name);
        }

        void ITracker.GetRates(TrackerCallback callback)
        {
            //This part of the interface is to be changed soon, so I do not implement it for now.
            //No part of the ETServer uses it anyway...
            throw new NotImplementedException();
        }

        void ITracker.GetRate(TrackerCallback callback)
        {
            callback(0, (int) et.GetFrameRate());
            throw new NotImplementedException();
        }

        void ITracker.SetRate(int rate, TrackerCallback callback)
        {
            if (et.EnumerateFrameRates().Contains((float)rate))
            {
                et.SetFrameRate((float)rate);
                callback(0, 1);
            }
            else
            {
                callback(1, 0);
            }

        }

        #endregion
    }
}
