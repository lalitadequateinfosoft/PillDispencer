using PillDispencer.Model;
using PillDispencer.Services;
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
using System.Windows.Shapes;

namespace PillDispencer.PopUp
{
    /// <summary>
    /// Interaction logic for DeviceInformationPopUp.xaml
    /// </summary>
    public partial class DeviceInformationPopUp : Window
    {
        private DeviceInfo deviceInfo;
        public bool Canceled { get; set; }
        public DeviceInformationPopUp()
        {
            InitializeComponent();
            deviceInfo = DeviceInformation.GetConnectedDevices();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Canceled = true;
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Canceled = false;
            Close();
        }
    }
}
