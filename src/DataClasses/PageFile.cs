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
    public class PageFile
    {
        public string MaximumSize { get; set; }
        public string InitialSize { get; set; }
        public string Name { get; set; }
    }
}
