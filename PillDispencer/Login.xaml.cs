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
using System.Windows.Shapes;

namespace PillDispencer
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        LoginViewModel loginViewModel;
        public Login()
        {
            loginViewModel=new LoginViewModel();
            InitializeComponent();
            this.DataContext = loginViewModel;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "you want to exit", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is LoginViewModel model)
            {
                if(!string.IsNullOrEmpty(model.Username) && !string.IsNullOrEmpty(model.Password))
                {
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                         
                }
            }
        }
    }
}
