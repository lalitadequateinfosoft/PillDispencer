using PillDispencer.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillDispencer.Model
{
   public class MachinesModel
    {
        public string DeviceId { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBit { get; set; }
        public int StopBit { get; set; }
        public int Parity { get; set; }
        public int SlaveAddress { get; set; }
        public bool IsActive { get; set; }
        public int MachineNo { get; set; }
        public string IsActiveText
        {
            get => IsActive==true?"ACTIVE":"IN ACTIVE";
        }
    }
}
