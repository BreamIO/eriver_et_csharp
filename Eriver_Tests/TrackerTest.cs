using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eriver.Trackers;
using Eriver.Network;

namespace Eriver_Tests
{
    [TestClass]
    public class TrackerTest
    {
        ITracker tracker;

        [TestInitialize]
        public void Setup()
        {
            AutoResetEvent done = new AutoResetEvent(false);
            tracker = TrackerFactory.GetTracker(1);
            while (!done.WaitOne(0))
            {
                tracker.GetState(delegate(int err, int res)
                {
                    if (err == 0)
                    {
                        done.Set();
                    }

                });
            }
        }

        public void TearDown()
        {
            tracker.Disable(null);
            tracker.EndCalibration(null);
            tracker = null;
        }

        [TestMethod]
        public void RegisterOnETEventTest()
        {
            ManualResetEvent recieved = new ManualResetEvent(false);
            GetPoint point = new GetPoint(-1, -1, -1);

            Assert.AreEqual(-1, point.X);
            Assert.AreEqual(-1, point.Y);
            Assert.AreEqual(-1, point.Timestamp);

            tracker.RegisterOnETEvent(delegate(GetPoint p)
            {
                point = p;
                recieved.Set();
            }
            );

            tracker.Enable(null);

            recieved.WaitOne();
            Assert.IsTrue(point.X >= 0);
            Assert.IsTrue(point.X <= 1);

            Assert.IsTrue(point.Y >= 0);
            Assert.IsTrue(point.Y <= 1);

            // Beware of this test. Depending on the implementation,
            // the "zero" point might be in the future. 
            // No implementation I know of has this, but the possibility exists.
            // If this is the case, disregard this test.
            Assert.IsTrue(point.Timestamp > 0);
        }

        [Ignore]
        [TestMethod]
        public void EnableTest()
        {

        }

        [Ignore]
        [TestMethod]
        public void DisableTest()
        {

        }

        [TestMethod]
        public void GetStateTest()
        {
            Assert.AreEqual(0, SyncGetState());

            tracker.Enable(null);
            Assert.AreEqual(1, SyncGetState());

            tracker.Disable(null);
            tracker.StartCalibration(0, null);
            Assert.AreEqual(2, SyncGetState());

            tracker.Enable(null);
            Assert.AreEqual(3, SyncGetState());
        }

        public int SyncGetState()
        {
            AutoResetEvent proceed = new AutoResetEvent(false);
            int state = -1;
            tracker.GetState(delegate(int error, int result)
            {
                if (error != 0) return;
                state = result;
                proceed.Set();
            });
            proceed.WaitOne();
            return state;
        }

        [TestMethod]
        public void StartCalibrationTest()
        {
            AutoResetEvent done = new AutoResetEvent(false);
            int error = -1;
            tracker.StartCalibration(0, delegate(int err, int result)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();
            Assert.AreEqual(0, error);
        }

        [TestMethod]
        public void EndCalibrationTest()
        {
            AutoResetEvent done = new AutoResetEvent(false);
            int error = 0;
            tracker.EndCalibration(delegate(int err, int res) {
                error = err;
                done.Set();
            });
            done.WaitOne();
            //Ending Calibration while not calibrating should result in an error.
            Assert.AreNotEqual(0, error);

            tracker.StartCalibration(0, delegate(int err, int res)
            {
                done.Set();
            });
            done.WaitOne();

            tracker.EndCalibration(delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();
            //Now we should be able to end calibration
            Assert.AreEqual(0, error);


            //We should be able to end calibration after adding some points.
            tracker.StartCalibration(0, delegate(int err, int res)
            {
                done.Set();
            });
            done.WaitOne();

            tracker.AddPoint(0.125, 0.999999, delegate(int err, int res)
            {
                done.Set();
            });
            done.WaitOne();

            tracker.AddPoint(0.8, 0.999999, delegate(int err, int res)
            {
                done.Set();
            });
            done.WaitOne();

            tracker.AddPoint(0.125, 0.1, delegate(int err, int res)
            {
                done.Set();
            });
            done.WaitOne();

            tracker.EndCalibration(delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();
            //Now we should be able to end calibration
            Assert.AreEqual(0, error);

        }

        [TestMethod]
        public void ClearCalibrationTest()
        {
            AutoResetEvent done = new AutoResetEvent(false);
            int error = 0;
            tracker.ClearCalibration(delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();
            //Ending Calibration while not calibrating should result in an error.
            Assert.AreNotEqual(0, error);

            tracker.StartCalibration(0, delegate(int err, int res)
            {
                done.Set();
            });
            done.WaitOne();

            tracker.ClearCalibration(delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();

            //Should not be an error if clearing an empty calibration.
            Assert.AreEqual(0, error);

            tracker.AddPoint(0.125, 0.999999, delegate(int err, int res)
            {
                done.Set();
            });
            done.WaitOne();

            //Test adding a few points.
            tracker.AddPoint(-0.125, 0.999999, delegate(int err, int res)
            {
                done.Set();
            });
            done.WaitOne();

            tracker.ClearCalibration(delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();

            //Should not be an error if clearing an this kind of calibration.
            Assert.AreEqual(0, error);
        }

        [TestMethod]
        public void AddPointTest()
        {
            AutoResetEvent done = new AutoResetEvent(false);
            int error = 0;
            tracker.AddPoint(0, 0, delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();
            Assert.AreNotEqual(0, error);

            tracker.StartCalibration(0, delegate(int err, int res)
            {
                done.Set();
            });
            done.WaitOne();

            //Test by adding several points, some of them outside the screen .
            tracker.AddPoint(0.125, 0.999999, delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();
            Assert.AreEqual(0, error);

            tracker.AddPoint(-0.125, 1.999999, delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();
            Assert.AreEqual(0, error);

            tracker.AddPoint(0.125, -0.999999, delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();
            Assert.AreEqual(0, error);

            tracker.AddPoint(-0.125, -0.999999, delegate(int err, int res)
            {
                error = err;
                done.Set();
            });
            done.WaitOne();
            Assert.AreEqual(0, error);
        }

        [Ignore]
        [TestMethod]
        public void GetNameTest()
        {

        }

        [Ignore]
        [TestMethod]
        public void GetRatesTest()
        {

        }

        [Ignore]
        [TestMethod]
        public void GetRateTest()
        {

        }

        [Ignore]
        [TestMethod]
        public void SetRateTest()
        {

        }
    }
}
