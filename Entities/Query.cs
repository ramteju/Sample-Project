using Entities.DTO;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Query
    {
        public Query()
        {
            Responses = new List<QueryResponse>();
        }
        public int Id { get; set; }
        public virtual Tan Tan { get; set; }
        [Required]
        public int TanId { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string Page { get; set; }
        public QueryType QueryType { get; set; }
        public virtual ApplicationUser PostedBy { get; set; }
        [Required]
        public string PostedById { get; set; }
        public DateTime Posted { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }
        public string UpdatedById { get; set; }
        public DateTime? Updated { get; set; }
        public virtual QueryWorkflowUser QueryWorkFlowUser { get; set; }
        [Required]
        public int QueryWorkFlowUserId { get; set; }
        public virtual ICollection<QueryResponse> Responses { get; set; }
    }
}
