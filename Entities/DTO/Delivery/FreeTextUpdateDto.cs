using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class FreeTextUpdateDto
    {
        public Guid Id { get; set; }
        public string FreeText { get; set; }
    }
    public class FreeTextBulkDto
    {
        public int BatchId { get; set; }
        public int CategoryId { get; set; }
        public List<FreeTextUpdateDto> Dtos { get; set; }
    }
}
