using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win_Info.DataClasses
{
    public class SystemInfo
    {
        // Basic Info
        public string SysName { get; set; }
        public string OS { get; set; }
        public string SystemType { get; set; }
        public string ChassisType { get; set; }
        public string BootTime { get; set; }
        public string SerialNumber { get; set; }
        // Advanced Info
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string InstallDate { get; set; }
        public string OSBuild { get; set; }
        public string OSVersion { get; set; }
        public string BiosVersion { get; set; }
        // Configuration
        public string Domain { get; set; }
        public string TimeZone { get; set; }
        public string Language { get; set; }
        public string BootDevice { get; set; }
        public string SystemDir { get; set; }
        //Memory
        public string TotalPhysicalMemory { get; set;}
        public string TotalVirtualMemory { get; set; }
        public string CommitLimit { get; set; }
        public string Committed { get; set; }
        public string AvailableMemory { get; set; }
        public string InUseMemory { get; set; }
    }
}
