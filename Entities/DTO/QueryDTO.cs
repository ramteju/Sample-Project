using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class QueryDTO
    {
        public int Id { get; set; }
        public int TanId { get; set; }
        public string TanNumber { get; set; }
        public string DocumentPath { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string Page { get; set; }
        public QueryType QueryType { get; set; }
    }
}
