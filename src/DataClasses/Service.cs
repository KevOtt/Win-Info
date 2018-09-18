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
    public class Service
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string State { get; set; }
        public string StartMode { get; set; }
    }
}
