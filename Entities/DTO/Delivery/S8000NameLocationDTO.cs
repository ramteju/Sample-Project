using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class S8000NameLocationDTO
    {
        public string TanNumber { get; set; }
        public string TanCategory { get; set; }
        public int TanSeries { get; set; }
        public string SubstanceName { get; set; }
        public string SubstanceLocation { get; set; }
        public string SubstanceOtherName { get; set; }
        public string SubstanceAuthorName { get; set; }
    }
}
