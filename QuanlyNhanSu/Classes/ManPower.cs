using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanlyNhanSu.Classes
{
    public class ManPower : Worker
    {
        public string RoleType { get; set; }
        public string MainRole { get; set; }
        public string BackupRole { get; set; }
        public string BackupRole2 { get; set; }
        public string NextRole { get; set; }
        public int RoleCapacity { get; set; }
        public int BackUpCapacity { get; set; }
        public int NextCapacity { get; set; }
        public string ShiftDisplay { get; set; }
    }
}
