using PillDispencer.Model;
using PillDispencer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private static readonly Regex _regex = new Regex("[^0-9-]+");
        public bool Canceled { get; set; }
        public DeviceInformationPopUp()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Canceled = true;
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if(!_regex.IsMatch(AddressBox.Text)
                || !_regex.IsMatch(GreenLight.Text)
                || !_regex.IsMatch(YellowLight.Text)
                || !_regex.IsMatch(RedLight.Text))
            {
                Canceled = false;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter valid input");
            }
            
        }
    }
}
