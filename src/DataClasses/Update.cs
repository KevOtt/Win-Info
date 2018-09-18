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
    public class Update
    {
        public string HotfixID { get; set; }
        public string InstalledOn { get; set; }
        public string Description { get; set; }
    }
}
