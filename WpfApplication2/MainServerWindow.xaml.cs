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
using Eriver.Trackers;

namespace Eriver.GUIServer
{
    /// <summary>
    /// Interaction logic for ServerGUI.xaml
    /// </summary>
    public partial class ServerGUI : Window
    {
        private Thread th;
        ETServer server;

        public Boolean InActive
        {
            get { return (Boolean)GetValue(InActiveProperty); }
            set { SetValue(InActiveProperty, value); }
        }

        public static readonly DependencyProperty InActiveProperty =
            DependencyProperty.Register("InActive", typeof(Boolean),
            typeof(ServerGUI), new UIPropertyMetadata(false));

        public ServerGUI()
        {
            th = null;
            server = null;
            Closing += Stop;
            InActive = true;
            InitializeComponent();
            this.DataContext = this;
            IdBox.Text = Eriver.GUIServer.Properties.Settings.Default.ID.ToString();
            TrackerType.SelectedItem = Properties.Settings.Default.TrackerType;
        }

        private void Stop(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConnectionList.DataContext = null;
            if (server != null)
                server.Dispose();
        }

        public void Start(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            try {
                server = new ETServer(Convert.ToByte(IdBox.Text), TrackerType.Text, Dispatcher);
                Properties.Settings.Default.ID = Convert.ToByte(IdBox.Text);
                Properties.Settings.Default.TrackerType = TrackerType.Text;
                Properties.Settings.Default.Save();
            } catch (Exception exc) {
                MessageBox.Show(exc.Message);  
                return;
            }

            InActive = false;
            th = new Thread(server.Start);
            ClickSwap(button, Start, Stop);
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
            //this.Resources["ButtonColor"] = Colors.LawnGreen;
            //this.Resources["ButtonText"] = "Start";

            ConnectionList.DataContext = null;
            //IdBox.IsEnabled = true;
            InActive = true;
        }

        private void ClickSwap(Button b, RoutedEventHandler handle1, RoutedEventHandler handle2)
        {
            b.Click -= handle1;
            b.Click += handle2;
        }

        private void Load_Profile_Click(object sender, RoutedEventArgs e)
        {
            Stream fStream;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = ".fish";
            ofd.DereferenceLinks = true;
            ofd.Title = "What profile do you want?";

            ofd.Filter = "Eriver Calibration Profile files (*.fish)|*.fish";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == true)
            {
                if ((fStream = ofd.OpenFile()) != null)
                {
                    try
                    {
                        var data = new byte[4096];
                        fStream.Read(data, 0, data.Length);
                        fStream.Close();
                        TrackerFactory.GetTracker(TrackerType.Text, 1).SetCalibration(data);
                    }
                    catch (IOException exception)
                    {
                        MessageBox.Show("Could not load profile.\n" + exception.Message);
                    }
                    catch (NotSupportedException)
                    {
                        MessageBox.Show("This tracker does not support this operation.");
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("A unknown error occured.\n" + exception.Message);
                    }
                }
            }
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
                    try
                    {
                        var c = TrackerFactory.GetTracker(TrackerType.Text, 1).GetCalibration();
                        fStream.Write(c, 0, c.Length);
                        fStream.Close();
                    }
                    catch (IOException exception)
                    {
                        MessageBox.Show("Could not save profile.\n" + exception.Message);
                    }
                    catch (NotSupportedException)
                    {
                        MessageBox.Show("This tracker does not support this operation.");
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("A unknown error occured.\n" + exception.Message);
                    }
                }
            }
        }

    }
}
