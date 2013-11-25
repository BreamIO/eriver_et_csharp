using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eriver.Network;
using EriverTrackers;

namespace Eriver.Trackers
{
    public delegate void ETEventHandler(GetPoint point);
    public delegate void TrackerCallback(int error, int result);

    public static class TrackerFactory
    {
        public static ITracker GetTracker()
        {
            return Create("Mock", 1);
        }

        public static ITracker GetTracker(byte id)
        {
            return Create("Mock", id);
        }

        public static ITracker GetTracker(string type, byte id)
        {
            return Create(type, id);
        }

        private static ITracker Create(string type, byte id) 
        {
            switch (type) {
                case "Tobii": return new TobiiTracker(id);
                default: return new MockTracker(id, 44);
            }
        }
    }

    public interface ITracker
    {

        /// <summary>
        /// Register an callback to be called when ETEvents is recieved from the tracker.
        /// Callback is called like this:
        /// callback(ETEvent)
        /// No arguments becuase I could not figure out how to store them properly.
        /// </summary>
        /// <param name="callback"></param>
        void RegisterOnETEvent(ETEventHandler callback);

    
        /// <summary>
        /// Set if the tracker should be active.
        /// callback is called with a result, *args and **kwargs when the operation is completed.
        /// </summary>
        /// <param name="callback"></param>
        void Enable(TrackerCallback callback);
    
        /// <summary>
        /// Set if the tracker should be inactive.
        /// This might shutdown the tracker if the implementation wants to.
        /// callback is called with a result, *args and **kwargs when the operation is completed.
        /// </summary>
        /// <param name="callback"></param>
         void Disable(TrackerCallback callback);

        /// <summary>
        /// Allows callers to query the tracker for a statuscode.
        /// Specific to implementations but some codes should be respected
        /// 0: Not enabled and not calibrating
        /// 1: Enabled and not calibrating
        /// 2: Calibrating but not enabled
        /// 3: Enabled and calibrating
        /// 
        /// Other than that, implementations may do what ever they feel fitting.
        /// callback is called with the status, *args and **kwargs.
        /// </summary>
        /// <param name="callback"></param>
        void GetState(TrackerCallback callback);

        /// <summary>
        /// Puts the tracker in calibration mode.
        /// If the tracker does not support calibration, return False.
        /// If the tracker could not be placed in calibration mode, return False.
        /// callback is called with a result when the operation is completed.
        /// </summary>
        /// <param name="callback"></param>
        void StartCalibration(TrackerCallback callback);

        /// <summary>
        /// Takes the tracker out of calibration mode.
        /// If the tracker could not be taken out of calibration mode, return False
        /// callback is called with a result when the operation is completed.
        /// </summary>
        /// <param name="callback"></param>
        void EndCalibration(TrackerCallback callback);

        /// <summary>
        /// Clears any calibration actions done.
        /// Restore the tracker to a state equal to that
        /// right after calibration was initiated.
        /// callback is called when the operation is completed.
        /// </summary>
        /// <param name="callback"></param>
        void ClearCalibration(TrackerCallback callback);

        /// <summary>
        /// Adds the point (x, y) to the calibration.
        /// When this is called, the user is expected to be looking at that point.
        /// callback is called with result when the operation is completed.
        /// result is False if the point could not be added.
        /// result is False if the tracker is not calibrating.
        /// Otherwise, result is True.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="callback"></param>
        void AddPoint(double x, double y, TrackerCallback callback);

        /// <summary>
        /// Fetches the current calibration data from the tracker.
        /// The tracker needs to include verification information 
        /// to later verify that the calibration is valid for it.
        /// </summary>
        /// <returns> The calibration data </returns>
        byte[] GetCalibration();
 
        /// <summary>
        /// Loads back the calibration data.
        /// No guarantees is given ragarding the validity of the data, 
        /// and needs to be checked before use.
        /// </summary>
        /// <param name="profile">Array of bytes containing a calibration profile</param>
        void SetCalibration(byte[] profile);

        /// <summary>
        /// Sets a XConfig on the tracker. 
        /// This allows the tracker to know some information about the Screen it is used with.
        /// The angle may be disregarded if not necessary, but it represents the angle
        /// between the normal vector of the users table and the tracker.
        /// </summary>
        /// <param name="angle">Angle given from calibrator</param>
        /// <param name="settings">XConfiguration data</param>
        void SetXConfig(XConfSettings settings, double angle);

        /// <summary>
        /// Free for interpretation of the implementor.
        /// callback is called with name when the operation is completed.
        /// </summary>
        /// <param name="callback"></param>
        void GetName(TrackerCallback callback);

        /// <summary>
        /// Gives a set of rates for which the tracker supports delivery of ETEvent.
        /// Common values include 24, 25 30, 60 and 120.
        /// Use -1 for unknown or variable rates.
        /// If -1 is returned, the implementation takes it upon itself to be able to handle all requested framerates.
        /// callback is called with rates when the operation is completed.
        /// </summary>
        /// <param name="callback"></param>
        void GetRates(TrackerCallback callback);

        /// <summary>
        /// Gives the current data rate of the tracker.
        /// </summary>
        /// <param name="callback"></param>
        void GetRate(TrackerCallback callback);

        /// <summary>
        /// Sets the tracker rate to the given value.
        /// The value given should be among those returned from GetRates, excluding -1.
        /// callback is called with result when the operation is completed.
        /// result is the rate that is set.
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="callback"></param>
        void SetRate(int rate, TrackerCallback callback);

    }
}
