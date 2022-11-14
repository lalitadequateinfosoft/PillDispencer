using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PillDispencer.ViewModel
{
    public class LoginViewModel:INotifyPropertyChanged
    {
        private ICommand loginCommand { get; set; }
        public ICommand LoginCommand
        {
            get
            {
                return loginCommand;
            }
            set
            {
                loginCommand = value;
            }
        }

        private bool canExecute = true;

        public LoginViewModel()
        {
            Username = "edward.2022";
            Password = "Test@123456";
        }
        private string _username;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        

        #region property changed event

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
