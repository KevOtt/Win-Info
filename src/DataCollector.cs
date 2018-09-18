/*
 Copyright (c) 2018 Kevin Ott
 Licensed under the MIT License
 See the LICENSE file in the project root for more information. 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Win_Info
{
    public class DataCollector
    {
        private ConnectionHandler connection;

        public DataCollector(ConnectionHandler connectionHandler)
        {
            if (connectionHandler == null)
            {
                throw new ArgumentNullException();
            }

            if (connectionHandler.validConnection == false)
            {
                throw new ArgumentException("Connection Handler Not Connected");
            }
            // Set local field for connections
            connection = connectionHandler;
        }

        public List<string> GetFeatureData()
        {
            ManagementObject[] managementObjectArray;
            // Create return list
            List<String> features = new List<string>();

            // Run query
            try
            {
                managementObjectArray = connection.RunQuery("SELECT NAME From Win32_ServerFeature");
            }
            catch (System.Management.ManagementException e)
            {
                // Catching invalid class exception for desktop OS's, returning empty list 
                if (e.Message == "Invalid class ")
                {
                    return features;
                }
                else throw;
            }
            
            // Return data from management object array
            foreach (ManagementObject m in managementObjectArray)
            {
                features.Add((m.GetPropertyValue("Name"))?.ToString() ?? "");
            }
            return features;
        }

        public List<DataClasses.CPU> GetCPUData()
        {
            // Run query
            ManagementObject[] managementObjectArray = connection.RunQuery("SELECT DeviceID, Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed, AddressWidth FROM win32_processor");
            // Create return list
            List<DataClasses.CPU> cpus = new List<DataClasses.CPU>();
            // Return data from management object array
            foreach (ManagementObject m in managementObjectArray)
            {
                cpus.Add(new DataClasses.CPU
                {
                    Name = (m.GetPropertyValue("Name"))?.ToString() ?? "",
                    DeviceID = (m.GetPropertyValue("DeviceID"))?.ToString() ?? "",
                    Cores = (m.GetPropertyValue("NumberOfCores")?.ToString() ?? ""),
                    LogicalProcessors = (m.GetPropertyValue("NumberOfLogicalProcessors")?.ToString() ?? ""),
                    AddressWidth = (m.GetPropertyValue("AddressWidth")?.ToString() ?? "" + "bit"),
                    MaxSpeed = (m.GetPropertyValue("MaxClockSpeed")?.ToString() ?? "" + "MHz")
                });
            }
            return cpus;
        }

        public List<DataClasses.Disk> GetDiskData()
        {
            // Run query
            ManagementObject[] managementObjectArray = connection.RunQuery("SELECT Name, Freespace, Size, Filesystem, VolumeName FROM Win32_LogicalDisk WHERE DriveType = 3");
            // Create return list
            List<DataClasses.Disk> disks = new List<DataClasses.Disk>();
            // Return data from management object array
            foreach (ManagementObject m in managementObjectArray)
            {
                disks.Add(new DataClasses.Disk
                {
                    Name = (m.GetPropertyValue("Name"))?.ToString() ?? "",
                    FreeSpace = (((UInt64)(m.GetPropertyValue("FreeSpace")) / 1074000000).ToString()) + " GB",
                    Size = (((UInt64)(m.GetPropertyValue("Size")) / 1074000000).ToString()) + " GB",
                    FileSystem = (m.GetPropertyValue("FileSystem"))?.ToString() ?? "",
                    VolumeName = (m.GetPropertyValue("VolumeName"))?.ToString() ?? ""
                });
            }
            return disks;
        }
 
        public List<DataClasses.NIC> GetNICData()
        {
            // Run query
            ManagementObject[] netAdapter = connection.RunQuery("SELECT Name, Manufacturer, MACAddress, PhysicalAdapter, NetEnabled, NetConnectionStatus FROM Win32_NetworkAdapter");
            ManagementObject[] netAdapterConfig = connection.RunQuery("SELECT MACAddress, IPAddress FROM Win32_NetworkAdapterConfiguration");
            
            // Create return list
            List<DataClasses.NIC> NICs = new List<DataClasses.NIC>();
            // Return data from management object array
            foreach (ManagementObject m in netAdapter)
            {
                string statusString = null;
                switch ((m.GetPropertyValue("NetConnectionStatus") == null ? 0 : ((UInt16)(m.GetPropertyValue("NetConnectionStatus")))))
                {
                    case 0:
                        statusString = "Disconnected";
                        break;
                    case 1:
                        statusString = "Connecting";
                        break;
                    case 2:
                        statusString = "Connected";
                        break;
                    case 3:
                        statusString = "Disconnecting";
                        break;
                    case 4:
                        statusString = "Hardware Not Present";
                        break;
                    case 5:
                        statusString = "Hardware Disabled";
                        break;
                    case 6:
                        statusString = "Hardware Malfunction";
                        break;
                    case 7:
                        statusString = "Media Disconnected";
                        break;
                    case 8:
                        statusString = "Authenticating";
                        break;
                    case 9:
                        statusString = "Authentication Succeeded";
                        break;
                    case 10:
                        statusString = "Authentication Failed";
                        break;
                    case 11:
                        statusString = "Invalid Address";
                        break;
                    case 12:
                        statusString = "Credentials Required";
                        break;
                    default:
                        statusString = "Unknown";
                        break;
                }
                NICs.Add(new DataClasses.NIC
                {
                    Name = (m.GetPropertyValue("Name") == null ? "" : (m.GetPropertyValue("Name"))?.ToString() ?? ""),
                    Manufacturer = (m.GetPropertyValue("Manufacturer") == null ? "" : (m.GetPropertyValue("Manufacturer"))?.ToString() ?? ""),
                    MACAddress = (m.GetPropertyValue("MACAddress") == null ? "" : (m.GetPropertyValue("MACAddress"))?.ToString() ?? ""),
                    PhysicalAdapter = (m.GetPropertyValue("PhysicalAdapter") == null ? false : (bool)(m.GetPropertyValue("PhysicalAdapter"))),
                    Enabled = (m.GetPropertyValue("NetEnabled") == null ? false : (bool)(m.GetPropertyValue("NetEnabled"))),
                    Status = statusString
                });
            };

            // Get IP address if it exists
            foreach(ManagementObject m in netAdapterConfig)
            {

                var mac = m.GetPropertyValue("MACAddress");
                string[] i = (string[])m.GetPropertyValue("IPAddress");
                
                if(mac == null || i == null)
                {
                    continue;
                }

                (NICs.First(n => n.MACAddress == mac.ToString())).IPAddrs = string.Join(" ; ",i);
            }

            return NICs;
        }

        public List<DataClasses.PageFile> GetPageFileData()
        {
            // Run query
            ManagementObject[] managementObjectArray = connection.RunQuery("SELECT Name, FileSize, InitialSize FROM Win32_PageFile");
            // Create return list
            List<DataClasses.PageFile> pageFiles = new List<DataClasses.PageFile>();
            // Return data from management object array
            foreach (ManagementObject m in managementObjectArray)
            {
                var t = (m.GetPropertyValue("InitialSize"));
                string initialsize = "SystemManaged";
                if (t.ToString() != "0")
                {
                    initialsize = (((UInt64)(t) / 1074000000).ToString()) + " GB";
                }

                pageFiles.Add(new DataClasses.PageFile
                {
                    Name = (m.GetPropertyValue("Name"))?.ToString() ?? "",
                    MaximumSize = (((UInt64)(m.GetPropertyValue("FileSize")) / 1074000000).ToString()) + " GB",
                    InitialSize = initialsize
                });
            }
            return pageFiles;
        }

        public List<DataClasses.PhysicalDisk> GetPhysicalDiskData()
        {
            // Run query
            ManagementObject[] managementObjectArray = connection.RunQuery("SELECT Name, Size, MediaType FROM Win32_DiskDrive");
            // Create return list
            List<DataClasses.PhysicalDisk> pageFiles = new List<DataClasses.PhysicalDisk>();
            // Return data from management object array
            foreach (ManagementObject m in managementObjectArray)
            {
                pageFiles.Add(new DataClasses.PhysicalDisk
                {
                    Name = (m.GetPropertyValue("Name"))?.ToString() ?? "",
                    Size = (((UInt64)(m.GetPropertyValue("Size")) / 1074000000).ToString()) + " GB",
                    MediaType = (m.GetPropertyValue("MediaType")?.ToString() ?? "")
                });
            }
            return pageFiles;

        }

        public List<DataClasses.Process> GetProcessData()
        {
            // Run query
            ManagementObject[] managementObjectArray = connection.RunQuery("Select Name, ProcessID, ThreadCount, PageFileUsage FROM Win32_Process");
            // Create return list
            List<DataClasses.Process> processes = new List<DataClasses.Process>();
            // Return data from management object array
            foreach (ManagementObject m in managementObjectArray)
            {
                processes.Add(new DataClasses.Process
                {
                    Name = (m.GetPropertyValue("Name"))?.ToString() ?? "",
                    ProcessID = (m.GetPropertyValue("ProcessID"))?.ToString() ?? "",
                    ThreadCount = (m.GetPropertyValue("ThreadCount"))?.ToString() ?? "",
                    PageFileUsage = (m.GetPropertyValue("PageFileUsage"))?.ToString() ?? ""
                });
            }
            return processes;
        }

        public List<DataClasses.Service> GetServiceData()
        {
            // Run query
            ManagementObject[] managementObjectArray = connection.RunQuery("Select Name, DisplayName, State, StartMode FROM Win32_Service");
            // Create return list
            List<DataClasses.Service> services = new List<DataClasses.Service>();
            // Return data from management object array
            foreach (ManagementObject m in managementObjectArray)
            {
                services.Add(new DataClasses.Service
                {
                    Name = (m.GetPropertyValue("Name"))?.ToString() ?? "",
                    DisplayName = (m.GetPropertyValue("DisplayName"))?.ToString() ?? "",
                    State = (m.GetPropertyValue("State"))?.ToString() ?? "",
                    StartMode = (m.GetPropertyValue("StartMode"))?.ToString() ?? ""
                });
            }
            return services;
        }

        public DataClasses.SystemInfo GetSystemInfo()
        {
            DataClasses.SystemInfo systeminfo = new DataClasses.SystemInfo();

            // Run queries for the four relevant WMI classes
            ManagementObject computerSystem = (connection.RunQuery("SELECT Name, SystemType, ChassisSKUNumber, Manufacturer, Model, Domain, TotalPhysicalMemory FROM Win32_ComputerSystem"))[0];
            ManagementObject operatingSystem = (connection.RunQuery("SELECT Caption, CSName, LastBootUpTime, InstallDate, BuildNumber, Version, " +
                "CurrentTimeZone, OSLanguage, BootDevice, SystemDirectory, TotalVirtualMemorySize FROM Win32_OperatingSystem"))[0];
            ManagementObject bios = (connection.RunQuery("SELECT Version, SerialNumber FROM Win32_Bios"))[0];
            ManagementObject perfOSMem = (connection.RunQuery("SELECT AvailableMBytes, CommittedBytes, CommitLimit FROM Win32_PerfFormattedData_PerfOS_Memory"))[0];
            ManagementObject timeZone = (connection.RunQuery("SELECT StandardName FROM win32_Timezone"))[0];

            // Format data and assign to return object
            systeminfo.AvailableMemory = perfOSMem.GetPropertyValue("AvailableMBytes")?.ToString() ?? "" + "MB";
            systeminfo.BiosVersion = bios.GetPropertyValue("Version")?.ToString() ?? "";
            systeminfo.BootDevice = operatingSystem.GetPropertyValue("BootDevice")?.ToString() ?? "";
            string boot = operatingSystem.GetPropertyValue("LastBootUpTime")?.ToString() ?? "";
            if (boot != null)
            {
                systeminfo.BootTime = ManagementDateTimeConverter.ToDateTime(boot).ToString("MM/dd/yyyy hh:mm:ss tt");
            }
            systeminfo.ChassisType = computerSystem.GetPropertyValue("ChassisSKUNumber")?.ToString() ?? "";
            systeminfo.CommitLimit = ((UInt64)(perfOSMem.GetPropertyValue("CommitLimit")) / 1048576).ToString() + "MB";
            systeminfo.Committed = ((UInt64)(perfOSMem.GetPropertyValue("CommittedBytes")) / 1048576).ToString() + "MB";
            systeminfo.Domain = computerSystem.GetPropertyValue("Domain")?.ToString() ?? "";
            systeminfo.InUseMemory = (((UInt64)(computerSystem.GetPropertyValue("TotalPhysicalMemory")) - (UInt64)(perfOSMem.GetPropertyValue("AvailableMBytes"))) / 1048576).ToString() + "MB";
            string install = operatingSystem.GetPropertyValue("InstallDate")?.ToString() ?? "";
            if(install != null)
            {
                systeminfo.InstallDate = ManagementDateTimeConverter.ToDateTime(install).ToString("MM/dd/yyyy hh:mm:ss tt");
            }
            systeminfo.Language = operatingSystem.GetPropertyValue("OSLanguage")?.ToString() ?? "";
            systeminfo.Manufacturer = computerSystem.GetPropertyValue("Manufacturer")?.ToString() ?? "";
            systeminfo.Model = computerSystem.GetPropertyValue("Model")?.ToString() ?? "";
            systeminfo.OS = operatingSystem.GetPropertyValue("Caption")?.ToString() ?? "";
            systeminfo.OSBuild = operatingSystem.GetPropertyValue("BuildNumber")?.ToString() ?? "";
            systeminfo.OSVersion = operatingSystem.GetPropertyValue("Version")?.ToString() ?? "";
            systeminfo.SerialNumber = bios.GetPropertyValue("SerialNumber")?.ToString() ?? "";
            systeminfo.SysName = operatingSystem.GetPropertyValue("CSName")?.ToString() ?? "";
            systeminfo.SystemDir = operatingSystem.GetPropertyValue("SystemDirectory")?.ToString() ?? "";
            systeminfo.SystemType = computerSystem.GetPropertyValue("SystemType")?.ToString() ?? "";
            systeminfo.TimeZone = timeZone.GetPropertyValue("StandardName")?.ToString() ?? "";
            systeminfo.TotalPhysicalMemory = ((UInt64)(computerSystem.GetPropertyValue("TotalPhysicalMemory")) / 1048576).ToString() + "MB";
            systeminfo.TotalVirtualMemory = ((UInt64)(operatingSystem.GetPropertyValue("TotalVirtualMemorySize")) / 1048576).ToString() + "MB";

            return systeminfo;
        }

        public List<DataClasses.Update> GetUpdateData()
        {
            // Run query
            ManagementObject[] managementObjectArray = connection.RunQuery("Select HotfixID, InstalledOn, Description FROM Win32_QuickFixEngineering");
            // Create return list
            List<DataClasses.Update> updates = new List<DataClasses.Update>();
            // Return data from management object array
            foreach (ManagementObject m in managementObjectArray)
            {
                updates.Add(new DataClasses.Update
                {
                    HotfixID = (m.GetPropertyValue("HotfixID"))?.ToString() ?? "",
                    InstalledOn = (m.GetPropertyValue("InstalledOn"))?.ToString() ?? "",
                    Description = (m.GetPropertyValue("Description"))?.ToString() ?? ""
                });
            }
            return updates;
        }

    }
}
