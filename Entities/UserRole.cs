using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core
{
    public class UserRole
    {
        public int Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        [Index("IX_UR", Order = 1, IsUnique = true)]
        public string UserId { get; set; }

        [Index("IX_UR", Order = 2, IsUnique = true)]
        public Role Role { get; set; }
    }
}