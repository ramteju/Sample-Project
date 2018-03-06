using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class QueryWorkflowUser
    {
        public int Id { get; set; }
        public virtual QueryWorkflow Workflow { get; set; }
        [Required]
        public int WorkflowId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public string UserId { get; set; }
        public QueryRole Role { get; set; }
    }
}
