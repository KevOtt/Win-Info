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
    public class Process
    {
        public string Name { get; set; }
        public string ProcessID { get; set; }
        public string ThreadCount { get; set; }
        public string PageFileUsage { get; set; }
    }
}
