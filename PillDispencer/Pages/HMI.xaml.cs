using PillDispencer.Model;
using PillDispencer.PopUp;
using PillDispencer.Services;
using PillDispencer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PillDispencer.Pages
{
    /// <summary>
    /// Interaction logic for HMI.xaml
    /// </summary>
    public partial class HMI : Page
    {
        CommunicationDevices weighing;
        CommunicationDevices control;
        private Dispatcher _dispathcer;

        MainWindow parentWindow;
        public MainWindow ParentWindow
        {
            get { return parentWindow; }
            set { parentWindow = value; }
        }
        HMIViewModel hMIViewModel;
        private DeviceInfo deviceInfo;
        public HMI()
        {
            hMIViewModel = new HMIViewModel();
            InitializeComponent();
            this.DataContext = hMIViewModel;
            _dispathcer = Dispatcher.CurrentDispatcher;
            weighing = new CommunicationDevices {
                IsConfigured=false
            };
            control = new CommunicationDevices
            {
                IsConfigured = false
            };
            deviceInfo = DeviceInformation.GetConnectedDevices();
        }


        #region page function
        private void Next_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveWeight_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Weight has been set");
        }

        private void ResetWeight_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is HMIViewModel model)
            {
                model.Weight = 0;
                MessageBox.Show("Weight has been reset");
            }
        }

        private void SaveTareWeight_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tare Weight has been set");
        }

        private void ResetTareWeight_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is HMIViewModel model)
            {
                model.TareWeight = 0;
                MessageBox.Show("Tare Weight has been reset");
            }
        }

        private void SaveCalibration_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Calibration has been set");
        }

        private void ResetCalibration_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is HMIViewModel model)
            {
                model.Zero = 0;
                model.Span = 0;
                MessageBox.Show("Tare Weight has been reset");
            }
        }

        #endregion


        #region Run function
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            if (!weighing.IsConfigured || !control.IsConfigured)
            {
                MessageBox.Show("Please configure Devices.");
                LoadSytem();
            }
            if(this.DataContext is HMIViewModel model)
            {
                model.IsNotRunning = false;
            }
            if (!weighing.IsConfigured || !control.IsConfigured)
            {
                MessageBox.Show("You have not configured devices.");
                return;
            }
            ExecuteLogic();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadSytem()
        {
            if (!weighing.IsConfigured)
            {
                ConfigurationDailog dailog = new ConfigurationDailog("Set Weight Device Configuration", deviceInfo.CustomDeviceInfos);
                dailog.ShowDialog();
                if (!dailog.Canceled)
                {

                    weighing.PortName = deviceInfo.CustomDeviceInfos.Where(x => x.DeviceID == dailog.DeviceId).FirstOrDefault().PortName;
                    weighing.DeviceId = dailog.DeviceId;
                    weighing.BaudRate = dailog.BaudRate;
                    weighing.DataBit = dailog.Databit;
                    weighing.StopBit = dailog.Stopbit;
                    weighing.Parity = dailog.ParityValue;
                    weighing.IsConfigured = true;
                }
            }

            if (!control.IsConfigured)
            {
                ConfigurationDailog dailog = new ConfigurationDailog("Set Control Card Configuration", deviceInfo.CustomDeviceInfos);
                dailog.ShowDialog();
                if (!dailog.Canceled)
                {

                    control.PortName = deviceInfo.CustomDeviceInfos.Where(x => x.DeviceID == dailog.DeviceId).FirstOrDefault().PortName;
                    control.DeviceId = dailog.DeviceId;
                    control.BaudRate = dailog.BaudRate;
                    control.DataBit = dailog.Databit;
                    control.StopBit = dailog.Stopbit;
                    control.Parity = dailog.ParityValue;
                    control.SlaveAddress = 4;


                    control.Green = new RegisterConfiguration
                    {
                        RType = 1,
                        RegisterNo = 1
                    };
                    control.Yellow = new RegisterConfiguration
                    {
                        RType = 1,
                        RegisterNo = 2
                    };
                    control.Yellow = new RegisterConfiguration
                    {
                        RType = 1,
                        RegisterNo = 3
                    };

                    control.IsConfigured = true;
                }
            }
        }

        private void ExecuteLogic()
        {

        }
        #endregion

        
    }
}
