using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eriver
{
    public delegate void ETEventHandler(ETEvent e);
    public delegate void TrackerCallback(int error, int result);

    interface Tracker
    {

    /**
    Register an callback to be called when ETEvents is recieved from the tracker.
    Callback is called like this:
    callback(ETEvent)
    No arguments becuase I could not figure out how to store them properly.
    */
    void register_onETEvent(ETEventHandler callback);

    
    /**
     * Set if the tracker should be active.
     * callback is called with a result, *args and **kwargs when the operation is completed.
     */
    void enable(TrackerCallback callback);
    
    /**
     * Set if the tracker should be inactive.
     * This might shutdown the tracker if the implementation wants to.
     * callback is called with a result, *args and **kwargs when the operation is completed.
     */
     void disable(TrackerCallback callback);

    /**
     * Allows callers to query the tracker for a statuscode.
     * Specific to implementations but some codes should be respected
     * 0: Not enabled and not calibrating
     * 1: Enabled and not calibrating
     * 2: Calibrating but not enabled
     * 3: Enabled and calibrating
     * 
     * Other than that, implementations may do what ever they feel fitting.
     * callback is called with the status, *args and **kwargs.
     */
    void getState(TrackerCallback callback);

    /*
     * Puts the tracker in calibration mode.
     * The angle may be disregarded if not necessary, but it represents the angle
     * between the normal vector of the users table and the tracker.
     * If the tracker does not support calibration, return False.
     * If the tracker could not be placed in calibration mode, return False.
     * callback is called with a result when the operation is completed.
     */
    void startCalibration(double angle, TrackerCallback callback);

    /*
     * Takes the tracker out of calibration mode.
     * If the tracker could not be taken out of calibration mode, return False
     * callback is called with a result when the operation is completed.
     */
    void endCalibration(TrackerCallback callback);

    /*
     * Clears any calibration actions done.
     * Restore the tracker to a state equal to that
     * right after calibration was initiated.
     * callback is called when the operation is completed.
     */
    void clearCalibration(TrackerCallback callback);

    /*
     * Adds the point (x, y) to the calibration.
     * When this is called, the user is expected to be looking at that point.
     * callback is called with result when the operation is completed.
     * result is False if the point could not be added.
     * result is False if the tracker is not calibrating.
     * Otherwise, result is True.
     */
    void addPoint(double x, double y, TrackerCallback callback);

    /*
     * Free for interpretation of the implementor.
     * callback is called with name when the operation is completed.
     */
    void getName(TrackerCallback callback);

    /*
     * Gives a set of rates for which the tracker supports delivery of ETEvent.
     * Common values include 24, 25 30, 60 and 120.
     * Use -1 for unknown or variable rates.
     * If -1 is returned, the implementation takes it upon itself to be able to handle all requested framerates.
     * callback is called with rates when the operation is completed.
     */
    void getRates(TrackerCallback callback);

    void getRate(TrackerCallback callback);

    /*
     * Sets the tracker rate to the given value.
     * The value given should be among those returned from getRates, excluding -1.
     * callback is called with result when the operation is completed.
     * result is the rate that is set.
     */
    void setRate(int rate, TrackerCallback callback);

    }
}
