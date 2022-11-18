using PillDispencer.Model;
using PillDispencer.ViewModel;
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

namespace PillDispencer.Pages
{
    /// <summary>
    /// Interaction logic for Machines.xaml
    /// </summary>
    public partial class Machines : Page
    {
        MainWindow parentWindow;
        public MainWindow ParentWindow
        {
            get { return parentWindow; }
            set { parentWindow = value; }
        }

        MachineViewModel machineViewModel;
        public Machines()
        {
            machineViewModel = new MachineViewModel();
            InitializeComponent();
            this.DataContext = machineViewModel;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is MachineViewModel model)
            {
                model.AddMachines(new MachinesModel
                {
                    DeviceId ="01",
                    PortName ="01",
                    BaudRate =13500,
                    DataBit =8,
                    StopBit =0,
                    Parity =0,
                    SlaveAddress =1,
                    IsActive =true
                });
            }
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            HMI hMI = new HMI();
            hMI.ParentWindow = ParentWindow;
            ParentWindow.frame.Navigate(hMI);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Image image = sender as Image;
                ContextMenu contextMenu = image.ContextMenu;
                contextMenu.PlacementTarget = image;
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if(this.DataContext is MachineViewModel model)
            {
                model.RemoveMachines(Convert.ToInt32(item.Tag));
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox=sender as CheckBox;
            if(this.DataContext is MachineViewModel model)
            {
                model.MachinesList.Where(x => x.MachineNo == Convert.ToInt32(checkbox.Tag)).ToList().ForEach(x => x.IsActive = true);
                model.LoadMachines();
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (this.DataContext is MachineViewModel model)
            {
                model.MachinesList.Where(x => x.MachineNo == Convert.ToInt32(checkbox.Tag)).ToList().ForEach(x => x.IsActive = false);
                model.LoadMachines();
            }
        }
    }
}
