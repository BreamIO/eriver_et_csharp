﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Eriver.Network;
using EriverTrackers;

namespace Eriver.Trackers
{
    class MockTracker : ITracker
    {

        List<ETEventHandler> onETEvent; //Should be List, but doesn't work for some reason...
        bool active;
        bool calibrating;
        byte name;
        int fps;
        double step;

        private static double dx;
        private static double dy;
        private static double aspectRatio;

        public MockTracker(byte name, int fps)
        {
            this.active=false;
            this.calibrating=false;
            this.name=name;
            this.fps = fps;
            step = 0.01;
            aspectRatio = 9.0 / 16.0;
            dx = 0;
            dy = 0;

            onETEvent = new List<ETEventHandler>();

            Thread thread = new Thread(new ThreadStart(this.run));
            thread.Start();
        }

        public void run()
        {
            foreach (GetPoint e in generateCircle())
            {
                foreach (ETEventHandler c in onETEvent)
                {
                    c(e);
                }
            }
        }

        #region Generators

        private IEnumerable<GetPoint> generateCircle()
        {
            double t = 0;
            while(true) 
            {
                yield return new GetPoint(0.5*Math.Cos(t)+0.5+dx, 0.5*aspectRatio*Math.Sin(t)+0.5+dy, (long)(DateTime.UtcNow-new DateTime (1970, 1, 1)).TotalMilliseconds);
                t+=step;
                Thread.Sleep(1000 / fps);
            }
        }

        #endregion

        #region Tracker Members

        void ITracker.RegisterOnETEvent(ETEventHandler callback)
        {
            onETEvent.Add(callback);
        }

        void ITracker.Enable(TrackerCallback callback)
        {
            active = true;
            if (callback != null)
                callback(0, 1);
        }

        void ITracker.Disable(TrackerCallback callback)
        {
            active = false;
            if (callback != null)
                callback(0, 1);
        }

        void ITracker.GetState(TrackerCallback callback)
        {
            int status = 0 + (active ? 1 : 0) + (calibrating ? 1 << 1 : 0);
            if (callback != null)
                callback(0, status);
        }

        void ITracker.StartCalibration(TrackerCallback callback)
        {
            calibrating = true;
            if (callback != null)
                callback(0, 1);
        }

        void ITracker.EndCalibration(TrackerCallback callback)
        {
            if (!calibrating)
            {
                if (callback != null)
                    callback(1, 0);
                return;
            }
            calibrating = false;
            if (callback != null)
                callback(0, 1);
        }

        void ITracker.ClearCalibration(TrackerCallback callback)
        {
            if (callback != null)
                callback(calibrating ? 0 : 1, 1);
        }

        void ITracker.AddPoint(double x, double y, TrackerCallback callback)
        {
            if (callback != null)
                callback(calibrating ? 0 : 1, 1);
        }

        byte[] ITracker.GetCalibration()
        {
            return new byte[] { 42, 13, 37, 47, 11 };
        }
        
        void ITracker.SetCalibration(byte[] profile)
        {
            throw new NotSupportedException("Feature not implemented");
        }

        void ITracker.GetName(TrackerCallback callback)
        {
            if (callback != null)
                callback(0, name);
        }

        void ITracker.GetRates(TrackerCallback callback)
        {
            throw new NotImplementedException();
        }

        void ITracker.GetRate(TrackerCallback callback)
        {
            if (callback != null)
                callback(0, fps);
        }

        void ITracker.SetRate(int rate, TrackerCallback callback)
        {
            fps = rate;
            if (callback != null)
                callback(0, fps);
        }

        void ITracker.SetXConfig(XConfSettings settings, double angle)
        {
            aspectRatio = settings.Height / settings.Width;
            dx = settings.Dx;
            dy = settings.Dy;
        }
        #endregion
    }
}
