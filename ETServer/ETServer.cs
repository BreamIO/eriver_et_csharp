using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using System.IO;

using Eriver.Trackers;

using Eriver.Network.Commands;
using System.Threading;

using log4net;
using log4net.Config;


namespace Eriver

{
    class ETServer : IDisposable
    {

        ManualResetEvent shutdown;
        byte name;
        ITracker tracker;
        ILog logger;
        string address;
        int port;

        public ETServer()
        {
            name = 1;
            Init();
        }

        public ETServer(byte id)
        {
            name = id;
            Init();
        }

        private void Init()
        {
            logger = LogManager.GetLogger("Eriver.ETServer");
            logger.Info("Starting the eye tracker server with id " + name);
            shutdown = new ManualResetEvent(false);
            tracker = TrackerFactory.GetTracker(name);
            address = "0.0.0.0";
            port = 3031;
        }

        public void Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(address), port);
            logger.Info("Listening on " + address + " : " + port);
            listener.Start();
            while (!shutdown.WaitOne(0))
            {
                TcpClient client = listener.AcceptTcpClient();
                Stream stream = client.GetStream();

                ConnectionHandler handler = new ConnectionHandler(name, stream, shutdown);
                Thread thread = new Thread(handler.Start);
                thread.Start();
            }
            listener.Stop();
        }

        public static void Main(string[] args)
        {
            ETServer server = new ETServer();
            server.Start();
        }

        #region IDisposable Members

        public void Dispose()
        {
            shutdown.Close();
        }

        #endregion
    }
}
