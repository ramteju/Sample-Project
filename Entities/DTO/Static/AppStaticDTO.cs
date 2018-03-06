using Entities.DTO.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class AppStaticDTO
    {
        public AppStaticDTO()
        {
            SolventBoilingPoints = new List<SolventBoilingPointDTO>();
            RegulerExpressions = new List<RegulerExpressionDTO>();
            CommentDictionary = new CommentDictionaryDTO();
            NamePriorities = new List<Entities.NamePriorities>();
        }
        public List<SolventBoilingPointDTO> SolventBoilingPoints;
        public List<RegulerExpressionDTO> RegulerExpressions;
        public CommentDictionaryDTO CommentDictionary { get; set; }
        public List<NamePriorities> NamePriorities;
    }

}
