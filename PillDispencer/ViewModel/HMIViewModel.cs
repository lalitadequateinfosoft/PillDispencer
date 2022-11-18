using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PillDispencer.ViewModel
{
    public class HMIViewModel : INotifyPropertyChanged
    {

        public HMIViewModel()
        {
            IsNotRunning = true;
            Zero = 0;
            Span = 0;
            Weight = 0;
            TareWeight = 0;
            CustomSetPoint1 = 0;
            CustomSetPoint2 = 0;
        }

        private Visibility iSRuning;
        public Visibility IsRunning
        {
            get => iSRuning;
            set
            {
                iSRuning = value;
                OnPropertyChanged(nameof(iSRuning));
            }
        }

        private Visibility iSStop;
        public Visibility IsStop
        {
            get => iSStop;
            set
            {
                iSStop = value;
                OnPropertyChanged(nameof(IsStop));
            }
        }

        private bool _isNotRunning;

        public bool IsNotRunning
        {
            get => _isNotRunning;
            set
            {
                _isNotRunning = value;
                OnPropertyChanged(nameof(IsNotRunning));
                if (_isNotRunning == true)
                {
                    IsRunning = Visibility.Visible;
                    IsStop = Visibility.Hidden;
                }
                else
                {
                    IsRunning = Visibility.Hidden;
                    IsStop = Visibility.Visible;
                }

            }
        }

        private decimal _Zero;
        public decimal Zero
        {
            get => _Zero;
            set
            {
                _Zero = value;
                OnPropertyChanged(nameof(Zero));
            }
        }

        private decimal _Span;
        public decimal Span
        {
            get => _Span;
            set
            {
                _Span = value;
                OnPropertyChanged(nameof(Span));
            }
        }

        private decimal _TareWeight;
        public decimal TareWeight
        {
            get => _TareWeight;
            set
            {
                _TareWeight = value;
                OnPropertyChanged(nameof(TareWeight));
            }
        }

        private decimal _Weight;
        public decimal Weight
        {
            get => _Weight;
            set
            {
                _Weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }

        private decimal _CustomSetPoint1;
        public decimal CustomSetPoint1
        {
            get => _CustomSetPoint1;
            set
            {
                _CustomSetPoint1 = value;
                OnPropertyChanged(nameof(CustomSetPoint1));
            }
        }
        private decimal _CustomSetPoint2;
        public decimal CustomSetPoint2
        {
            get => _CustomSetPoint2;
            set
            {
                _CustomSetPoint2 = value;
                OnPropertyChanged(nameof(CustomSetPoint2));
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
