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
            if (this.DataContext is LoginViewModel model)
            {
                if (!string.IsNullOrEmpty(model.Username) && !string.IsNullOrEmpty(model.Password) && model.Username== "edward.2022" && model.Password== "Adk@2017")
                {
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Login failed!,invalid/Empty username and password provided");
                }
            }
        }

        private void txtPass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext is LoginViewModel model)
            {
                var box=sender as TextBox;
                if(string.IsNullOrEmpty(box.Text.ToString()))
                {
                    model.Password = string.Empty;
                    model.HPassword= string.Empty;
                    return;
                }

                model.Password= box.Text.ToString();
                var text=box.Text.ToString();   
                model.HPassword= new string('*', text.Length);
            }
        }
    }
}
