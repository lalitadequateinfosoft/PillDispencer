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
            IsSaveEnabled=true;
            CalculateSpan = false;
            AutoZeroEnabled=false;
        }

        

        private bool _isNotRunning;

        public bool IsNotRunning
        {
            get => _isNotRunning;
            set
            {
                _isNotRunning = value;
                OnPropertyChanged(nameof(IsNotRunning));
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

        public bool IsRunning
        {
            get => !_isNotRunning;
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


        private bool _CustomSetPointChecked1;
        public bool CustomSetPointChecked1
        {
            get => _CustomSetPointChecked1;
            set
            {
                _CustomSetPointChecked1 = value;
                OnPropertyChanged(nameof(CustomSetPointChecked1));
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


        private bool _CustomSetPointChecked2;
        public bool CustomSetPointChecked2
        {
            get => _CustomSetPointChecked2;
            set
            {
                _CustomSetPointChecked2 = value;
                OnPropertyChanged(nameof(CustomSetPointChecked2));
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

        private bool _HundChecked;

        public bool HundChecked
        {
            get => _HundChecked;
            set
            {
                _HundChecked = value;
                OnPropertyChanged(nameof(HundChecked));
            }
        }

        private bool _FifChecked;

        public bool FifChecked
        {
            get => _FifChecked;
            set
            {
                _FifChecked = value;
                OnPropertyChanged(nameof(FifChecked));
            }
        }

        private bool _TweChecked;

        public bool TweChecked
        {
            get => _TweChecked;
            set
            {
                _TweChecked = value;
                OnPropertyChanged(nameof(TweChecked));
            }
        }

        private bool _TenChecked;

        public bool TenChecked
        {
            get => _TenChecked;
            set
            {
                _TenChecked = value;
                OnPropertyChanged(nameof(TenChecked));
            }
        }

        private bool _FivChecked;

        public bool FivChecked
        {
            get => _FivChecked;
            set
            {
                _FivChecked = value;
                OnPropertyChanged(nameof(FivChecked));
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
