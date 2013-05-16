using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using System.IO;

using Eriver.Trackers;

using Eriver.Network;
using System.Threading;

using log4net;
using log4net.Config;
using System.Configuration;


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
            //XmlConfigurator.Configure();
            logger = LogManager.GetLogger("Eriver.ETServer");
            log4net.ThreadContext.Properties["id"] = "Id: " + name;
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
                ConnectionHandler handler = new ConnectionHandler(name, "<unavaliable>", stream, shutdown);
                Thread thread = new Thread(handler.Start);
                thread.Start();
            }
            listener.Stop();
        }

        public static void Main()
        {
            ETServer server = new ETServer();
            server.Start();
            server.shutdown.WaitOne();
            server.Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            shutdown.Close();
        }

        #endregion
    }
}
