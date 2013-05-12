using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Eriver.Network;
using log4net;

namespace Eriver
{
    class ConnectionHandler : IDisposable
    {
        private byte name;
        private string clientId;
        private Stream stream;
        private ManualResetEvent shutdown;
        private ManualResetEvent stop;
        private ILog logger;

        public ConnectionHandler(byte name, string clientId, Stream stream, ManualResetEvent shutdown)
        {
            // TODO: Complete member initialization
            this.name = name;
            this.clientId = clientId;

            logger = LogManager.GetLogger(this.GetType());
            log4net.ThreadContext.Properties["id"] = "Id: " + clientId;

            this.stream = stream;
            this.shutdown = shutdown;
            this.stop = new ManualResetEvent(false);
        }

        public void Start()
        {
            using (log4net.ThreadContext.Stacks["NDC"].Push("Run"))
            {
                Run();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void Run()
        {
            EriverStreamReaderWriter unmarshaller = new EriverStreamReaderWriter(stream);
            EriverProtocol prot;
            while (!shutdown.WaitOne(0) && !stop.WaitOne(0))
            {
                try
                {
                    prot = unmarshaller.Read();
                    logger.Debug(prot);
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
