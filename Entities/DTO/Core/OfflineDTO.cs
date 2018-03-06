using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO.Core
{
    public class OfflineDTO
    {
        public string DocumentReviewedUserId { get; set; }
        public DateTime DocumentReadStartTime { get; set; }
        public DateTime DocumentReadEndTime { get; set; }
        public string TanDocumentKeyPath { get; set; }
        public string TotalPaths { get; set; }
        public string TanData { get; set; }
    }
}
