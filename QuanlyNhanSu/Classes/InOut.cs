using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanlyNhanSu.Classes
{
    public class InOut : Worker
    {
        public DateTime Date { get; set; }
        public string In_Work { get; set; }
        public string Out_Work { get; set; }
        public string Line { get; set; }
        public string In_Line { get; set; }
        public string Out_Line { get; set; }
        public string RowColor { get; set; }
    }
}
