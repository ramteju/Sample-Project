using Newtonsoft.Json;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class TanInfoDTO
    {
        public Tan Tan { get; set; }
        public TanData TanData { get; set; }
        public string TanWiseKeyWords { get; set; }
    }
}
