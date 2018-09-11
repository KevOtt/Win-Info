using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win_Info.DataClasses
{
    public class CPU
    {
        public string DeviceID { get; set; }
        public string Name { get; set; }
        public string Cores { get; set; }
        public string LogicalProcessors { get; set; }
        public string MaxSpeed { get; set; }
        public string AddressWidth { get; set; }
    }
}
