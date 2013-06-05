using System;
using System.IO;
using Eriver.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eriver_Tests
{
    [TestClass]
    public class NetworkTest
    {
        EriverStreamReaderWriter esrw;

        [TestInitialize]
        public void SetUp()
        {
            esrw = new EriverStreamReaderWriter(new MemoryStream(255));
        }

        /// <summary>
        /// Test to ensure that writing and reading from the ReadWriter works as intended.
        /// Tests by writing test values to the testee, and then attempting to read them back.
        /// </summary>
        [TestMethod]
        public void ReadWriteTest()
        {
            foreach (Command cmd in Enum.GetValues(typeof(Command)))
            {
                SetUp();
                EriverProtocol ep = CreateTestProtocol(cmd);
                esrw.Write(ep);
                EriverProtocol new_ep = esrw.Read();

                Assert.AreEqual(ep, new_ep);
                Assert.AreEqual<EriverProtocol>(ep, new_ep);
            }
        }

        [TestMethod]
        public void TransformToGetPointProtocolTest()
        {
            EriverProtocol ep = CreateTestProtocol(Command.GetPoint);

            byte[] testData = EriverStreamReaderWriter.Transform(ep);
            Assert.AreEqual(ep, EriverStreamReaderWriter.Transform(testData));

        }

        [TestMethod]
        public void TransformToStartCalProtocolTest()
        {
            EriverProtocol ep = CreateTestProtocol(Command.StartCalibration);

            byte[] testData = EriverStreamReaderWriter.Transform(ep);
            Assert.AreEqual<EriverProtocol>(ep, EriverStreamReaderWriter.Transform(testData));
        }

        [TestMethod]
        public void TransformToEndCalProtocolTest()
        {
            EriverProtocol ep = CreateTestProtocol(Command.EndCalibration);

            byte[] testData = EriverStreamReaderWriter.Transform(ep);
            Assert.AreEqual<EriverProtocol>(ep, EriverStreamReaderWriter.Transform(testData));
        }

        [TestMethod]
        public void TransformToClearCalProtocolTest()
        {
            EriverProtocol ep = CreateTestProtocol(Command.ClearCalibration);

            byte[] testData = EriverStreamReaderWriter.Transform(ep);
            Assert.AreEqual<EriverProtocol>(ep, EriverStreamReaderWriter.Transform(testData));
        }

        [TestMethod]
        public void TransformToAddPointProtocolTest()
        {
            EriverProtocol ep = CreateTestProtocol(Command.AddPoint);

            byte[] testData = EriverStreamReaderWriter.Transform(ep);
            Assert.AreEqual<EriverProtocol>(ep, EriverStreamReaderWriter.Transform(testData));
        }

        [TestMethod]
        public void TransformToUnavailableProtocolTest()
        {
            EriverProtocol ep = CreateTestProtocol(Command.Unavailable);

            byte[] testData = EriverStreamReaderWriter.Transform(ep);
            Assert.AreEqual<EriverProtocol>(ep, EriverStreamReaderWriter.Transform(testData));
        }

        [TestMethod]
        public void TransformToNameProtocolTest()
        {
            EriverProtocol ep = CreateTestProtocol(Command.Name);

            byte[] testData = EriverStreamReaderWriter.Transform(ep);
            Assert.AreEqual<EriverProtocol>(ep, EriverStreamReaderWriter.Transform(testData));
        }

        [TestMethod]
        public void TransformToFpsProtocolTest()
        {
            EriverProtocol ep = CreateTestProtocol(Command.Fps);

            byte[] testData = EriverStreamReaderWriter.Transform(ep);
            Assert.AreEqual<EriverProtocol>(ep, EriverStreamReaderWriter.Transform(testData));
        }

        [TestMethod]
        public void TransformToKeepAliveProtocolTest()
        {
            EriverProtocol ep = CreateTestProtocol(Command.KeepAlive);

            byte[] testData = EriverStreamReaderWriter.Transform(ep);
            Assert.AreEqual<EriverProtocol>(ep, EriverStreamReaderWriter.Transform(testData));

        }

        private EriverProtocol CreateTestProtocol(Command command)
        {
            EriverProtocol ep = new EriverProtocol();
            ep.Kind = command;

            ep.GetPoint = new GetPoint();
            ep.GetPoint.X = 0.314;
            ep.GetPoint.Y = 0.431;
            ep.GetPoint.Timestamp = 1;

            ep.AddPoint = new AddPoint();
            ep.AddPoint.X = 0.431;
            ep.AddPoint.Y = 0.314;

            ep.StartCalibration = new StartCalibration();
            ep.StartCalibration.Angle = 13.37;

            ep.Fps = new Fps();
            ep.Fps.Value = 4041;

            ep.Name = new Name();
            ep.Name.Value = 33;

            return ep;
        }

        [TestMethod]
        public void TransformFromEmptyProtocolTest()
        {
            EriverProtocol ep = new EriverProtocol();
            byte[] ans = EriverStreamReaderWriter.Transform(ep);

            Assert.AreEqual<int>(1, ans.Length);
            CollectionAssert.AreEqual(new byte[] { 0 }, ans);

        }

        [TestMethod]
        public void TransformFromGetPointProtocolTest()
        {
            Command underTest = Command.GetPoint;
            EriverProtocol ep = CreateTestProtocol(Command.GetPoint);

            byte[] ans = EriverStreamReaderWriter.Transform(ep);

            Assert.AreEqual<int>(CommandConvert.ToLength(underTest) + 1, ans.Length);
            CollectionAssert.AreEqual(new byte[] { (byte) underTest,
                0x3F, 0xD4, 0x18, 0x93, 0x74, 0xBC, 0x6A, 0x7F,
                0x3F, 0xDB, 0x95, 0x81, 0x06, 0x24, 0xDD, 0x2F,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01}, ans);

        }

        [TestMethod]
        public void TransformFromAddPointProtocolTest()
        {
            Command underTest = Command.AddPoint;
            EriverProtocol ep = CreateTestProtocol(Command.AddPoint);

            byte[] ans = EriverStreamReaderWriter.Transform(ep);

            Assert.AreEqual<int>(CommandConvert.ToLength(underTest) + 1, ans.Length);
            CollectionAssert.AreEqual(new byte[] { (byte) underTest,
                0x3F, 0xDB, 0x95, 0x81, 0x06, 0x24, 0xDD, 0x2F,
                0x3F, 0xD4, 0x18, 0x93, 0x74, 0xBC, 0x6A, 0x7F}, ans);

        }

        [TestMethod]
        public void TransformFromStartCalProtocolTest()
        {
            Command underTest = Command.StartCalibration;
            EriverProtocol ep = CreateTestProtocol(underTest);

            byte[] ans = EriverStreamReaderWriter.Transform(ep);

            Assert.AreEqual<int>(CommandConvert.ToLength(underTest) + 1, ans.Length);
            CollectionAssert.AreEqual(new byte[] { (byte) underTest,
                0x40, 0x2A, 0xBD, 0x70, 0xA3, 0xD7, 0x0A, 0x3D}, ans);
        }

        [TestMethod]
        public void TransformFromFPSProtocolTest()
        {
            Command underTest = Command.Fps;
            EriverProtocol ep = CreateTestProtocol(underTest);

            byte[] ans = EriverStreamReaderWriter.Transform(ep);

            Assert.AreEqual<int>(CommandConvert.ToLength(underTest) + 1, ans.Length);
            CollectionAssert.AreEqual(new byte[] { (byte) underTest,
                0x00, 0x00, 0x0F, 0xC9}, ans);
        }

        [TestMethod]
        public void TransformFromNameProtocolTest()
        {
            Command underTest = Command.Name;
            EriverProtocol ep = CreateTestProtocol(underTest);

            byte[] ans = EriverStreamReaderWriter.Transform(ep);

            Assert.AreEqual<int>(CommandConvert.ToLength(underTest) + 1, ans.Length);
            CollectionAssert.AreEqual(new byte[] { (byte)underTest, 0x21 }, ans);
        }



    }
}
