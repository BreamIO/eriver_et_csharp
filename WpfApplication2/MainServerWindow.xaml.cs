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
using System.Threading;
using Microsoft.Win32;
using System.IO;

namespace Eriver.GUIServer
{
    /// <summary>
    /// Interaction logic for ServerGUI.xaml
    /// </summary>
    public partial class ServerGUI : Window
    {
        Thread th;
        ETServer server;

        public ServerGUI()
        {
            th = null;
            server = null;
            Closing += Stop;
            InitializeComponent();
        }

        private void Stop(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConnectionList.DataContext = null;
            server.Dispose();
        }

        public void Start(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            try {
                server = new ETServer(Convert.ToByte(IdBox.Text), TrackerType.Text, Dispatcher);
            } catch (Exception exc) {
                MessageBox.Show(exc.Message);  
                return;
            }

            IdBox.IsEnabled = false;
            th = new Thread(server.Start);
            ClickSwap(button, Start, Stop);
            this.Resources["ButtonColor"] = Colors.Red;
            this.Resources["ButtonText"] = "Stop";
            ConnectionList.DataContext = server;
            try
            {
                th.Start();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error Occured", MessageBoxButton.OK, MessageBoxImage.Error);
                Stop(sender, new RoutedEventArgs());
            }
        }

        public void Stop(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            server.shutdown.Set();
            server.Dispose();

            ClickSwap(button, Stop, Start);
            this.Resources["ButtonColor"] = Colors.LawnGreen;
            this.Resources["ButtonText"] = "Start";

            ConnectionList.DataContext = null;
            IdBox.IsEnabled = true;
        }

        private void ClickSwap(Button b, RoutedEventHandler handle1, RoutedEventHandler handle2)
        {
            b.Click -= handle1;
            b.Click += handle2;
        }

        private void Load_Profile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Loading is not yet implemented. :(");
        }

        private void Save_Profile_Click(object sender, RoutedEventArgs e)
        {
            Stream fStream;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.DefaultExt = ".fish";
            sfd.DereferenceLinks = true;
            sfd.OverwritePrompt = true;
            sfd.Title = "What do you want the profile to be called?";

            sfd.Filter = "Eriver Calibration Profile files (*.fish)|*.fish";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == true)
            {
                if ((fStream = sfd.OpenFile()) != null)
                {
                    fStream.WriteByte(42);
                    fStream.Close();
                }
            }
        }
    }
}
