using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO.Static
{
    public class CommentDictionaryDTO
    {
        public CommentDictionaryDTO()
        {
            CVTs = new List<CVT>();
            Freetexts = new List<FreeText>();
        }
        public List<CVT> CVTs { get; set; }
        public List<FreeText> Freetexts { get; set; }
    }
}
