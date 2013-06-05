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

namespace Eriver.GUIServer
{

    public delegate void StatusChangedHandler(object sender, EventArgs args);

    class ConnectionHandler : IDisposable
    {
        private byte name;
        private Stream stream;
        private EriverStreamReaderWriter readerWriter;
        private ManualResetEvent shutdown;
        private ManualResetEvent stop;
        private ILog logger;
        private ITracker tracker;
        public bool Listen { get; set; }

        public event StatusChangedHandler OnStatusChanged;

        public ConnectionHandler(byte name, string clientId, Stream stream, ManualResetEvent shutdown)
        {
            // TODO: Complete member initialization
            this.name = name;

            logger = LogManager.GetLogger(this.GetType());
            log4net.ThreadContext.Properties["id"] = "Id: " + clientId;

            this.stream = stream;
            readerWriter = new EriverStreamReaderWriter(stream);
            this.shutdown = shutdown;
            this.stop = new ManualResetEvent(false);
            tracker = TrackerFactory.GetTracker(name);
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
                EriverProtocol proto = new EriverProtocol();
                proto.Kind = Command.GetPoint;
                proto.GetPoint = point;
                Send(proto);
            });
            
            tracker.Enable(null);
            using (log4net.ThreadContext.Stacks["NDC"].Push("Run"))
            {
                Run();
            }
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

            while (!shutdown.WaitOne(0) && !stop.WaitOne(0))
            {
                try
                {
                    prot = readerWriter.Read();
                    using (log4net.ThreadContext.Stacks["NDC"].Push("ConnHandler_Handlers"))
                    {
                        var m = ConnHandler_Handlers.Messages[CommandConvert.ToByte(prot.Kind)]();
                        logger.Debug("Read packet: " + prot);
                        m.Accept(this, prot);
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
        }

        public void Send(EriverProtocol proto)
        {
            logger.Debug("Writing packet: " + proto);
            byte[] buf = EriverStreamReaderWriter.Transform(proto);
            try
            {
                stream.Write(buf, 0, buf.Length);
            }
            catch (IOException e)
            {
                logger.Error("IOException while writing to stream. Closing handler.");
                logger.Debug(e);
                stop.Set();
            }
        }

        public ITracker GetTracker()
        {
            return tracker;
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
