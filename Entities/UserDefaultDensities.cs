using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserDefaultDensities
    {
        public int Id { get; set; }
        public virtual UserRole UserRole { get; set; }
        [ForeignKey("UserRole")]
        public int UserRoleId { get; set; }
        public float UserRXNDensity { get; set; }
    }
}
