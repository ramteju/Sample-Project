using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ReactionDTO
    {
        public Guid Id { get; set; }
        public int TanId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class StageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ReactionId { get; set; }
        public int TanId { get; set; }
        public int DisplayOrder { get; set; }

    }

    public class SelectedReactionDto
    {
        public List<RSDDTO> Participants { get; set; }
        public List<RSNDTO> Comments { get; set; }
        public List<ConditionsDTO> Conditions { get; set; }
    }
}
