using JetBrains.Annotations;
using PillDispencer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PillDispencer.ViewModel
{
    public class MachineViewModel : INotifyPropertyChanged
    {

        public MachineViewModel()
        {
            MachinesList = new List<MachinesModel>();
            Machines = new ObservableCollection<MachinesModel>();
        }

        private List<MachinesModel> _machineslist;

        public List<MachinesModel> MachinesList
        {
            get => _machineslist;
            set
            {
                _machineslist = value;
                OnPropertyChanged(nameof(MachinesList));
            }
        }

        private ObservableCollection<MachinesModel> _machines;

        public ObservableCollection<MachinesModel> Machines
        {
            get => _machines;
            set
            {
                _machines = value;
                OnPropertyChanged(nameof(Machines));
            }
        }

        public void AddMachines(MachinesModel model)
        {
            model.MachineNo = MachinesList.Count() + 1;
            MachinesList.Add(model);
            LoadMachines();
        }

        public void RemoveMachines(int machine)
        {
            foreach (var item in MachinesList.ToList())
            {
                if (item.MachineNo == machine)
                {
                    MachinesList.Remove(item);
                }
            }
            LoadMachines();
        }

        public void LoadMachines()
        {
            Machines = new ObservableCollection<MachinesModel>();
            foreach (var item in MachinesList.ToList().OrderBy(x=>x.MachineNo))
            {
                Machines.Add(item);
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
