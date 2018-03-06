using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class MoveTansDTO
    {
        public List<int> TanIds { get; set; }
        public int TargetCategory { get; set; }
    }
}
