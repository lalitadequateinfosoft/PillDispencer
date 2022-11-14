using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillDispencer
{
     public class Model
    {
        public int Value { get; set; }
        public string Image { get; set; }
        public Model(string image, int value)
        {
            Image = image;
            Value = value;

        }
    }
    
}
