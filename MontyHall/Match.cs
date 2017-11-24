using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MontyHall
{
    public class Match
    {
        public int Round { get; set; }
        public int Doors { get; set; }
        public bool Changed { get; set; }
        public string Result { get; set; }
    }
}
