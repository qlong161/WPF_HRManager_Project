using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanlyNhanSu.Classes
{
    public class Worker
    {
        public bool IsSelected { get; set; } 
        public string WN { get; set; }     
        public string Name { get; set; }     
        public string Department { get; set; } 
        public string Shift { get; set; }    
        public int Level { get; set; }       
        public string StartDate { get; set; }
        public string TotalYears { get; set; } 
        public string Status { get; set; } = "Active";
    }
}
