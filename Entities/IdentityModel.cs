using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool? IsOnline { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
        public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; }
        public virtual ICollection<Query> Queries { get; set; }
        public virtual ICollection<QueryResponse> Responses { get; set; }
        public int? AnalystId { get; set; }
        public float? BenchMark { get; set; }
        public bool? AllowedForCuration { get; set; }
        public bool? AllowedForReview { get; set; }
        public bool? AllowedForQC { get; set; }
    }

    public class ApplicationRoles : IdentityRole
    {
        public Role Role { get; set; }
    }

    public class ApplicationRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PriorityOrder { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
