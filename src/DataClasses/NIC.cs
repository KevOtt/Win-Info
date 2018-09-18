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

namespace Win_Info.DataClasses
{
    public class NIC
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string MACAddress { get; set; }
        public bool PhysicalAdapter { get; set; }
        public bool Enabled { get; set; }
        public string Status { get; set; }
        public string IPAddrs { get; set; }
    }
}
