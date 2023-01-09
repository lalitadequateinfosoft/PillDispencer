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
            IsBatchComplete = false;
            Zero = 0;
            Span = 0;
            Weight = 0;
            TareWeight = 0;
            IsSaveEnabled = true;
            CalculateSpan = false;
            AutoZeroEnabled = false;
            SetPoint0 = 0;
            SetPoint0Percent = 0;
            IsSetPoint0Passed = false;
            SetPoint1 = 0;
            SetPoint1Percent = 0;
            IsSetPoint1Passed = false;
            SetPoint2 = 0;
            SetPoint2Percent = 0;
            IsSetPoint2Passed = false;
        }



        private bool _isNotRunning;

        public bool IsNotRunning
        {
            get => _isNotRunning;
            set
            {
                _isNotRunning = value;
                IsRunning = !value;
                OnPropertyChanged(nameof(IsNotRunning));
            }
        }

        private decimal _setPoint0Percent;
        public decimal SetPoint0Percent
        {
            get => _setPoint0Percent;
            set
            {
                _setPoint0Percent = value;
                OnPropertyChanged(nameof(SetPoint0Percent));
            }
        }

        private decimal _setPoint0;
        public decimal SetPoint0
        {
            get => _setPoint0;
            set
            {
                _setPoint0 = value;
                OnPropertyChanged(nameof(SetPoint0));
            }
        }

        private bool _isSetPoint0Passed;

        public bool IsSetPoint0Passed
        {
            get => _isSetPoint0Passed;
            set
            {
                _isSetPoint0Passed = value;
                OnPropertyChanged(nameof(IsSetPoint0Passed));
            }
        }

        private decimal _setPoint1Percent;
        public decimal SetPoint1Percent
        {
            get => _setPoint1Percent;
            set
            {
                _setPoint1Percent = value;
                OnPropertyChanged(nameof(SetPoint1Percent));
            }
        }

        private decimal _setPoint1;
        public decimal SetPoint1
        {
            get => _setPoint1;
            set
            {
                _setPoint1 = value;
                OnPropertyChanged(nameof(SetPoint1));
            }
        }
        private bool _isSetPoint1Passed;

        public bool IsSetPoint1Passed
        {
            get => _isSetPoint1Passed;
            set
            {
                _isSetPoint1Passed = value;
                OnPropertyChanged(nameof(IsSetPoint1Passed));
            }
        }

        private decimal _setPoint2Percent;
        public decimal SetPoint2Percent
        {
            get => _setPoint2Percent;
            set
            {
                _setPoint2Percent = value;
                OnPropertyChanged(nameof(SetPoint2Percent));
            }
        }

        private decimal _setPoint2;
        public decimal SetPoint2
        {
            get => _setPoint2;
            set
            {
                _setPoint2 = value;
                OnPropertyChanged(nameof(SetPoint2));
            }
        }

        private bool _isSetPoint2Passed;

        public bool IsSetPoint2Passed
        {
            get => _isSetPoint2Passed;
            set
            {
                _isSetPoint2Passed = value;
                OnPropertyChanged(nameof(IsSetPoint2Passed));
            }
        }

        private bool _isSaveEnabled;

        public bool IsSaveEnabled
        {
            get => _isSaveEnabled;
            set
            {
                _isSaveEnabled = value;
                OnPropertyChanged(nameof(IsSaveEnabled));
            }
        }
        private bool _autoZeroEnabled;

        public bool AutoZeroEnabled
        {
            get => _autoZeroEnabled;
            set
            {
                _autoZeroEnabled = value;
                OnPropertyChanged(nameof(AutoZeroEnabled));
            }
        }

        private bool _CalculateSpan;

        public bool CalculateSpan
        {
            get => _CalculateSpan;
            set
            {
                _CalculateSpan = value;
                OnPropertyChanged(nameof(CalculateSpan));
            }
        }

        
        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }


        private bool _isBatchComplete;

        public bool IsBatchComplete
        {
            get => _isBatchComplete;
            set
            {
                _isBatchComplete = value;
                OnPropertyChanged(nameof(IsBatchComplete));
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

        private decimal _factor;
        public decimal Factor
        {
            get => _factor;
            set
            {
                _factor = value;
                OnPropertyChanged(nameof(Factor));
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

        private decimal _TareWeightPercentage;
        public decimal TareWeightPercentage
        {
            get => _TareWeightPercentage;
            set
            {
                _TareWeightPercentage = value;
                OnPropertyChanged(nameof(TareWeightPercentage));
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
        private decimal _ActualWeight;
        public decimal ActualWeight
        {
            get => _ActualWeight;
            set
            {
                _ActualWeight = value;
                OnPropertyChanged(nameof(ActualWeight));
            }
        }

        private decimal _WeightPercentage;
        public decimal WeightPercentage
        {
            get => _WeightPercentage;
            set
            {
                _WeightPercentage = value;
                OnPropertyChanged(nameof(WeightPercentage));
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
