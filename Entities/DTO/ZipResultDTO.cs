﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
   public class ZipResultDTO
    {
        public HashSet<string> TanNumbers { get; set; }
        public string Path { get; set; }
        public int Count { get; set; }
    }
}
