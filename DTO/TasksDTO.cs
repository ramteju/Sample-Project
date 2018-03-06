using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string TanName { get; set; }
        public int NUMsCount { get; set; }
        public int RXNsCount { get; set; }
        public string Shipment { get; set; }
        public string Analyst { get; set; }
        public string Status { get; set; }
        public int BatchNo { get; set; }
    }
}
