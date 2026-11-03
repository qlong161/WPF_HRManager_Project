using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanlyNhanSu.Classes
{
    public class WaveShift : Worker
    {
        public DateTime Date { get; set; }
        public int Shift_Duration { get; set; }
        public int Break_Duration { get; set; }
    }
}
