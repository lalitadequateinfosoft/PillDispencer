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
using PillDispencer.Model;
using onvif.soapenv;
using System.Text.RegularExpressions;
using PillDispencer.Services.IServices;
using Microsoft.VisualStudio.Threading;
namespace PillDispencer
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        LoginViewModel loginViewModel;
        private readonly string sessionId;
        public Login()
        {
            loginViewModel = new LoginViewModel();
            InitializeComponent();
            this.DataContext = loginViewModel;
            sessionId = "SignInSession_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            if(AppicationSession.SessionDetails!=null)
            {
                MainWindow main = new MainWindow();
                this.Close();
                main.Show();
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "you want to exit", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel model)
            {
                try
                {
                    model.IsReady = false;
                    if (!string.IsNullOrEmpty(model.Username) && !string.IsNullOrEmpty(model.Password))
                    {
                        LoginModel user = new LoginModel();
                        user.Email = model.Username.ToLower().ToString();
                        user.Password = model.Password;
                        ApiService service = new ApiService();
                        UserTokens tokens = await service.LoginAsync(user);
                        if (tokens.Code == (int)ResponseEnum.success)
                        {
                            string log = "Login with username:" + model.Username + " is successfull.";
                            LogWriter.LogWrite(log, sessionId);
                            
                            MainWindow main = new MainWindow();
                            this.Close();
                            model.IsReady = true;
                            main.Show();
                        }
                        else
                        {
                            string log = "Sign in with username:" + model.Username + " failed. invalid/Empty username and password provided";
                            LogWriter.LogWrite(log, sessionId);
                            MessageBox.Show("Login failed!,invalid/Empty username and password provided");
                        }
                            
                    }
                    else
                    {
                        string log = "Empty username/password provided..";
                        LogWriter.LogWrite(log, sessionId);
                        MessageBox.Show("Login failed!,Empty username/password provided..");
                    }
                    model.IsReady = true;
                }
                catch(TaskCanceledException ex)
                {
                    model.IsReady = true;
                    string log = "An error has occured:\r"+ex.StackTrace.ToString()+".";
                    log = log + "\r\n error description : " + ex.ToString();
                    LogWriter.LogWrite(log, sessionId);
                }

                
            }
        }

        private void HiddenPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.DataContext is LoginViewModel model)
                {
                    if (model.IsPasswordHidden == Visibility.Visible)
                    {
                        model.Password = ((PasswordBox)sender).Password;
                        VisiblePass.Text = model.Password;
                    }
                }
            }
            catch(Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }
            
        }

        private void VisiblePass_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.DataContext is LoginViewModel model)
                {
                    if (model.IsPasswordHidden == Visibility.Collapsed)
                    {
                        model.Password = ((TextBox)sender).Text;
                        HiddenPass.Password = model.Password;
                    }
                }
            }
            catch(Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }
        }

        private void passMode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.DataContext is LoginViewModel model)
                {
                    if (model.IsPasswordVisible == Visibility.Visible)
                    {
                        string hiddenpass = HiddenPass.Password;
                        string plainPass = VisiblePass.Text;

                        model.IsPasswordVisible = Visibility.Collapsed;
                        model.IsPasswordHidden = Visibility.Visible;

                        passMode.Icon = FontAwesome.WPF.FontAwesomeIcon.EyeSlash;
                        HiddenPass.Focusable = true;
                        HiddenPass.Focus();
                    }
                    else
                    {
                        string hiddenpass = HiddenPass.Password;
                        string plainPass = VisiblePass.Text;

                        model.IsPasswordVisible = Visibility.Visible;
                        model.IsPasswordHidden = Visibility.Collapsed;

                        passMode.Icon = FontAwesome.WPF.FontAwesomeIcon.Eye;
                        VisiblePass.Focusable = true;
                        VisiblePass.Focus();
                    }
                }
            }
            catch(Exception ex)
            {
                string log = "An error has occured:\r" + ex.StackTrace.ToString() + ".";
                log = log + "\r\n error description : " + ex.ToString();
                LogWriter.LogWrite(log, sessionId);
            }
            
        }
    }
}
