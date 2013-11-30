using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Eriver.Network;
using log4net;
using Eriver.Trackers;
using System.Net.Sockets;
using EriverTrackers;

namespace Eriver.GUIServer
{

    public delegate void StatusChangedHandler(object sender, EventArgs args);

    class ConnectionHandler : IDisposable
    {
        private byte name;
        private Stream stream;
        private EriverStreamReaderWriter readerWriter;
        private ManualResetEvent stop;
        private ILog logger;
        private ITracker tracker;
        public bool Listen { get; set; }
        public ITracker Tracker {
            get { return tracker; }
        }
        public String Description { get; set; }

        public event StatusChangedHandler OnStatusChanged;

        /// <summary>
        /// Handler of connection to the ETServer
        /// Reads data from the stream, and answers accordingly.
        /// Deals with the tracker aswell using the ITracker interface.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tracker_type"></param>
        /// <param name="clientId"></param>
        /// <param name="stream"></param>
        /// <param name="description"></param>
        public ConnectionHandler(byte name, string tracker_type, string clientId, NetworkStream stream, string description)
        {
            this.name = name;

            logger = LogManager.GetLogger(this.GetType());
            log4net.ThreadContext.Properties["id"] = "Id: " + clientId;

            this.stream = stream;
            readerWriter = new EriverStreamReaderWriter(stream);
            this.stop = new ManualResetEvent(false);
            tracker = TrackerFactory.GetTracker(tracker_type, name);
            Description = description;
        }

        public void Start()
        {
            tracker.RegisterOnETEvent(delegate(GetPoint point)
            {
                if (stop.WaitOne(0))
                {
                    OnStatusChanged(this, new EventArgs());
                    tracker.Disable(null);
                    Thread.CurrentThread.Abort(); // Shutting down this thread.
                    return;
                }
                if (Listen) {
                    EriverProtocol proto = new EriverProtocol();
                    proto.Kind = Command.GetPoint;
                    proto.GetPoint = point;
                    Send(proto);
                }
            });
            
            tracker.Enable(null);
            using (log4net.ThreadContext.Stacks["NDC"].Push("Run"))
            {
                Run();
            }

            //Finalize calibration if disconnecting during calibration.
            tracker.GetState(delegate(int state, int error)
            {
                if ((state & 2) != 0)
                {
                    tracker.ClearCalibration(delegate(int res, int err)
                    {
                        tracker.EndCalibration(null);
                    });
                    
                }
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void Run()
        {
            
            EriverProtocol prot = new EriverProtocol();

            // Sending name first according to spec.
            prot.Kind = Command.Name;
            prot.Name = new Name();
            prot.Name.Value = name;
            Send(prot);

            while (!stop.WaitOne(0))
            {
                try
                {
                    prot = readerWriter.Read();
                    using (log4net.ThreadContext.Stacks["NDC"].Push("ConnHandler_Handlers"))
                    {
                        logger.Debug("Read packet: " + prot);
                        try
                        {
                            var m = ConnHandler_Handlers.Messages[CommandConvert.ToByte(prot.Kind)]();
                            m.Accept(this, prot);
                        }
                        catch (KeyNotFoundException)
                        {
                            logger.Error("Package type not supported. Different versions?");
                            stop.Set();
                            break;
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    using (log4net.ThreadContext.Stacks["NDC"].Push("InvalidOperationException"))
                    {
                        stop.Set();
                        logger.Error("Error in stream.");
                        logger.Debug(e);
                    }
                }

            }
            stream.Close();
        }

        public void Send(EriverProtocol proto)
        {
            logger.Debug("Writing packet: " + proto);
            byte[] buf = EriverStreamReaderWriter.Transform(proto);
            try
            {
                if (stream != null)
                    stream.Write(buf, 0, buf.Length);
            }
            catch (IOException e)
            {
                logger.Error("IOException while writing to stream. Closing handler.");
                logger.Debug(e);
                stop.Set();
            }
            catch (ObjectDisposedException e)
            {
                logger.Error("ObjectDisposedException while writing to stream. Closing handler.");
                logger.Debug(e);
                stop.Set();
            }
        }

        public ITracker GetTracker()
        {
            return tracker;
        }

        public void Kill() {
            stop.Set();
            tracker.Disable(null);
        }

        #region IDisposable Members

        public void Dispose()
        {
            logger.Info("Disposing object.");
            stop.Set();
            stop.Close();
            throw new NotImplementedException();
        }

        #endregion
    }
}
