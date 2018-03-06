using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTracking.Models.Core
{
    public class RoleWiseTanHistory
    {
        public int Id { get; set; }
        public virtual UserRole UserRole { get; set; }
        [ForeignKey("UserRole")]
        [Index("IX_UserRole_tanState", IsUnique = true, Order = 1)]
        public int UserRoleId { get; set; }
        [Index("IX_UserRole_tanState", IsUnique = true, Order = 2)]
        public TanState TanState { get; set; }
        public String Data { get; set; }
        public DateTime? LostModifiedDate { get; set; }

    }
}
