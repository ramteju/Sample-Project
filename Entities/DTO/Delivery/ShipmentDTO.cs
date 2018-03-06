using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class BatchDTO
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public string DocumentsPath { get; set; }
        public DateTime DateCreated { get; set; }
        public int TanCount { get; set; }
    }
}
