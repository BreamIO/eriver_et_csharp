using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Eriver
{
    class ConnectionHandler : IDisposable
    {
        private byte name;
        private Stream stream;
        private ManualResetEvent shutdown;
        private ManualResetEvent stop;

        public ConnectionHandler(byte name, Stream stream, ManualResetEvent shutdown)
        {
            // TODO: Complete member initialization
            this.name = name;
            this.stream = stream;
            this.shutdown = shutdown;
            this.stop = new ManualResetEvent(false);
        }

        public void Start()
        {
            Run();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void Run()
        {

        }

        #region IDisposable Members

        public void Dispose()
        {
            stop.Set();
            stop.Close();
            throw new NotImplementedException();
        }

        #endregion
    }
}
