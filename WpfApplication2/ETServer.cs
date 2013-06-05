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
using System.Collections.ObjectModel;
using System.Windows.Threading;


namespace Eriver.GUIServer

{
    class ETServer : IDisposable
    {

        public ManualResetEvent shutdown { get; set; }
        byte name;
        ILog logger;
        string address;
        int port;
        TcpListener listener;
        private Dispatcher dispatcher;
        public ObservableCollection<ConnectionHandler> Connections { get; set; }

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

        public ETServer(byte id, Dispatcher disp)
        {
            dispatcher = disp;
            name = id;
            Init();
        }

        private void Init()
        {
            //XmlConfigurator.Configure();
            logger = LogManager.GetLogger(this.GetType());
            log4net.ThreadContext.Properties["id"] = "Id: " + name;
            Connections = new ObservableCollection<ConnectionHandler>();
            logger.Info("Starting the eye tracker server with id " + name);
            shutdown = new ManualResetEvent(false);
            address = "0.0.0.0";
            port = 4041;
            listener = null;
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Parse(address), port);
            logger.Info("Listening on " + address + " : " + port);
            listener.Start();
            
            while (!shutdown.WaitOne(0))
            {
                TcpClient client = null;
                try
                {
                    client = listener.AcceptTcpClient();
                }
                catch (SocketException exc)
                {
                    logger.Error("Error on accept. Probably due to stopping server.");
                    logger.Error(exc);
                    return;
                }
                
                Stream stream = client.GetStream();
                ConnectionHandler handler = new ConnectionHandler(name, "<unavaliable>", stream, shutdown);
                handler.OnStatusChanged += delegate(object sender, EventArgs args)
                {
                    dispatcher.BeginInvoke(new Action(delegate() { Connections.Remove((ConnectionHandler)sender); }));
                };
                dispatcher.BeginInvoke(new Action(delegate() { Connections.Add(handler); }));
                Thread thread = new Thread(handler.Start);
                thread.Start();
            }
            listener.Stop();
            listener = null;
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
            if (listener != null)
                listener.Stop();
            shutdown.Close();
        }

        #endregion
    }
}
