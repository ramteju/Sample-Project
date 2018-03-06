using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core
{
    public class AppSettings
    {
        public int Id { get; set; }
        public int MaxTasks { get; set; }
        public bool Autoallocation { get; set; }
    }
}