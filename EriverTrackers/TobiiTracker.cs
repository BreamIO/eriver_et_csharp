using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tobii.EyeTracking.IO;
using Eriver.Network;
using EriverTrackers;

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

        private void connect()
        {
            EyeTrackerBrowser browser = new EyeTrackerBrowser(EventThreadingOptions.BackgroundThread);
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
                et = e.EyeTrackerInfo.Factory.CreateEyeTracker(EventThreadingOptions.BackgroundThread);

                //Setup event handlers for this tracker.
                et.CalibrationStarted += et_CalibrationStarted;
                et.CalibrationStopped += et_CalibrationStopped;
                et.ConnectionError += et_ConnectionError;
                et.GazeDataReceived += et_GazeDataRecieved;
                if (enabled)
                {
                    et.StartTracking();
                }
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
            int leftValidity = gazeDataItem.LeftValidity;
            int rightValidity = gazeDataItem.RightValidity;
            Point2D left = gazeDataItem.LeftGazePoint2D;
            Point2D right = gazeDataItem.RightGazePoint2D;
            GetPoint answer = new GetPoint();

            if (leftValidity == BEST_VALIDITY && rightValidity == BEST_VALIDITY)
            {
                eye_distance = new Point2D(right.X - left.X, right.Y - left.Y);
            }
            else if (leftValidity == BEST_VALIDITY)
            {
                right.X = left.X + eye_distance.X;
                right.Y = left.Y + eye_distance.Y;
            }
            else if (rightValidity == BEST_VALIDITY)
            {
                left.X = right.X - eye_distance.X;
                left.Y = right.Y - eye_distance.Y;
            }
            else
            {

            }

            answer.X = (left.X + right.X) / 2;
            answer.Y = (left.Y + right.Y) / 2;
            return answer;
        }

        #region Tracker Members

        void ITracker.SetXConfig(XConfSettings xconfig, double angle)
        {
            //Do XConfig
            //XConfSettings xconf = new XConfSettings(512, 290, 0, 20, 3, 2); //Dreamhack values
            /* Read from file in future.
            try 
            {
                using(Open("eriver.xconf", "r") as File f) {
                    XConfSettings.load(xconf, f);
                }
            }
            catch (IOError e) 
            {
                Console.WriteLine("No XConf file. Continuing without...");
            }
            */

            double[] x= new double[]{(-xconfig.Width)/2 + xconfig.Dx, xconfig.Width/2 - xconfig.Dx, (-xconfig.Width)/2 + xconfig.Dx};
            double[] y= new double[]{xconfig.Height+xconfig.Dy, xconfig.Height+xconfig.Dy, xconfig.Dy};
            double[] z= new double[]{xconfig.Dz, xconfig.Dz, xconfig.Dz};

            double theta = Math.PI * ((angle + xconfig.Dangle) / 180);

            XConfiguration xconf = new XConfiguration();

            xconf.UpperLeft  = new Point3D( x[0], Math.Cos(theta)*y[0] - Math.Sin(theta)*z[0], Math.Sin(theta)*y[0] + Math.Cos(theta)*z[0]);
            xconf.UpperRight = new Point3D( x[1], Math.Cos(theta)*y[1] - Math.Sin(theta)*z[1], Math.Sin(theta)*y[1] + Math.Cos(theta)*z[1]);
            xconf.LowerLeft  = new Point3D( x[2], Math.Cos(theta)*y[2] - Math.Sin(theta)*z[2], Math.Sin(theta)*y[2] + Math.Cos(theta)*z[2]);

            et.SetXConfiguration(xconf); //(UpperLeft, UpperRight, LowerLeft);
        }

        void ITracker.RegisterOnETEvent(ETEventHandler callback)
        {
            EtEventHandlers += callback;
        }

        void ITracker.Enable(TrackerCallback callback)
        {
            enabled = true;
            if (et != null)
            {
                et.StartTracking();
            }

            if (callback != null)
                callback(0, 1);
        }

        void ITracker.Disable(TrackerCallback callback)
        {
            enabled = false;

            if (et != null)
                et.StopTracking();

            if (callback != null)
                callback(0, 1);
        }

        void ITracker.GetState(TrackerCallback callback)
        {
            if (et == null)
            {
                if (callback != null)
                    callback(1, -1);
                return;
            }
            int status = (enabled ? 1 : 0) + (calibrating ? 1 << 1 : 0);
            if (callback != null)
                callback(0, status);
        }

        void ITracker.StartCalibration(TrackerCallback callback)
        {
            if (et == null)
            {
                callback(1, 0);
                return;
            }
            if (calibrating)
            {
                callback(1, 0);
                return;
            }
            try
            {
                //setXconfig(angle);
                et.StartCalibration();
            }
            catch (EyeTrackerException e)
            {
                if (callback != null)
                {
                    callback(e.ErrorCode, 0);
                }
                return;
            }
            calibrating = true;
            if (callback != null)
                callback(0, 1);
        }

        void ITracker.EndCalibration(TrackerCallback callback)
        {
            if (et == null)
            {
                if (callback != null)
                    callback(1, 0);
                return;
            }

            if (!calibrating)
            {
                callback(1, 0);
                return;
            }

            try
            {
                et.ComputeCalibration();
            }
            catch (EyeTrackerException e)
            {
                //If error is not OPERATION_FAILED
                if (e.ErrorCode != 0x20000502)
                {
                    if (callback != null)
                    {
                        //We tell them of the error
                        callback(e.ErrorCode, 0);
                    }
                    return;
                }
                //Or we continue and let it stop calibrating without 
                // calculating anything.
            }

            try
            {
                et.StopCalibration();
            }
            catch (EyeTrackerException e)
            {
                if (callback != null)
                {
                    callback(e.ErrorCode, 0);
                }
                return;
            }

            calibrating = false;
            if (callback != null)
                callback(0, 1);
        }

        void ITracker.ClearCalibration(TrackerCallback callback)
        {
            if (et == null)
            {
                if (callback != null)
                    callback(1, 0);
                return;
            }

            if (!calibrating)
            {
                callback(1, 0);
                return;
            }

            try
            {
                et.ClearCalibration();
            }
            catch (EyeTrackerException e)
            {
                if (callback != null)
                {
                    callback(e.ErrorCode, 0);
                }
                return;
            }

            if (callback != null)
                callback(0, 1);
        }

        void ITracker.AddPoint(double x, double y, TrackerCallback callback)
        {
            if (et == null)
            {
                if (callback != null)
                    callback(1, 0);
                return;
            }

            if (!calibrating)
            {
                callback(1, 0);
                return;
            }

            try
            {
                et.AddCalibrationPoint(new Point2D(x, y));
            }
            catch (EyeTrackerException e)
            {
                if (callback != null)
                {
                    callback(e.ErrorCode, 0);
                }
                return;
            }

            if (callback != null)
                callback(0, 1);
        }

        void ITracker.GetName(TrackerCallback callback)
        {
            if (callback != null)
                callback(0, name);
        }

        void ITracker.GetRates(TrackerCallback callback)
        {
            //This part of the interface is to be changed soon, so I do not implement it for now.
            //No part of the ETServer uses it anyway...
            throw new NotImplementedException();
        }

        byte[] ITracker.GetCalibration()
        {
            Calibration c = et.GetCalibration();
            return c.RawData;
        }

        void ITracker.SetCalibration(byte[] profile)
        {
            Calibration c = new Calibration(profile);
            et.SetCalibration(c);
        }

        void ITracker.GetRate(TrackerCallback callback)
        {
            if (callback == null) return;
            if (et == null)
            {
                if (callback != null)
                    callback(1, 0);
                return;
            }

            try
            {
                callback(0, (int)et.GetFrameRate());
            }
            catch (EyeTrackerException e)
            {
                callback(e.ErrorCode, 0);
            }
        }

        void ITracker.SetRate(int rate, TrackerCallback callback)
        {
            if (et.EnumerateFrameRates().Contains((float)rate))
            {
                if (et == null)
                {
                    if (callback != null)
                        callback(1, 0);
                    return;
                }
                et.SetFrameRate((float)rate);
                if (callback != null)
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
