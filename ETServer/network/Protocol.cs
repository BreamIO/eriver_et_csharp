using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace eriver.network
{
    public delegate void EriverProtocolDelegate(object sender, ProtocolArgs args);

    class Protocol
    {

        private Socket socket;

        public event EriverProtocolDelegate GetPoint;
        public event EriverProtocolDelegate StartCal;
        public event EriverProtocolDelegate EndCal;
        public event EriverProtocolDelegate ClearCal;
        public event EriverProtocolDelegate AddPoint;
        public event EriverProtocolDelegate Unavaliable;
        public event EriverProtocolDelegate Name;
        public event EriverProtocolDelegate Fps;
        public event EriverProtocolDelegate KeepAlive;

        public Protocol(Socket s) {
            this.socket = s;
        }

        private void run()
        {
            while (true) 
            {
                Command cmd = readCommand();
                ProtocolArgs args = cmd.Deserialize(socket);
                Invoke(, args)
            }
        }

        #region Invokers

        // Invoke the GetPoint event; called whenever list changes
        protected virtual void Invoke(EriverProtocolDelegate e, ProtocolArgs args)
        {
            if (e != null) 
                e(this, args);
        }

        #endregion

        #region Readers

        private Command readCommand()
        {
            throw new NotImplementedException();
        }

        private int readInt()
        {
 	        throw new NotImplementedException();
        }

        private long readLong()
        {
 	        throw new NotImplementedException();
        }

        private double readDouble()
        {
 	        throw new NotImplementedException();
        }

        #endregion

        private void SendPacket(Command c, ProtocolArgs args)
        {
            byte[] packet = byte[c.Length];
            Buffer.BlockCopy()
        }
    }
}
