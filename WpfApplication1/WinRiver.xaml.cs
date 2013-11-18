using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Eriver.Network;
using System.Net.Sockets;
using System.Threading;

namespace Eriver.Winriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        EriverStreamReaderWriter esrw;
        TcpClient client = null;
        GetPoint CurrentPoint { get; set; }
        Thread bgThread;

        public MainWindow()
        {
            CurrentPoint = new GetPoint(10, 20, 0);
            bgThread = new Thread(handle);
            bgThread.Start();
            InitializeComponent();
        }

        private void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Event! Connect to " + Host.Text.ToString() + ":" + Port.Text.ToString());
            if (client != null)
            {
                esrw = null;
                client.Close();
                client = null;
            }

            try
            {
                client = new TcpClient(Host.Text.ToString(), Convert.ToInt32(Port.Text.ToString()));
            }
            catch (SocketException)
            {
                MessageBox.Show("Error connecting to host. Please check that the other side is listening for requests on the port number.");
                return;
            }
            esrw = new EriverStreamReaderWriter(client.GetStream());
        }
       

        private void MovePointer(double x, double y)
        {
            Canvas.SetTop(TrackerPoint, y);
            Canvas.SetLeft(TrackerPoint, x);
        }

        private void handle()
        {
            while (true)
            {
                if (esrw != null)
                {
                    EriverProtocol ep = esrw.Read();
                    if (ep.Kind == Command.GetPoint)
                        MovePointer(ep.GetPoint.X, ep.GetPoint.Y);
                }
            }
        }


    }
}
