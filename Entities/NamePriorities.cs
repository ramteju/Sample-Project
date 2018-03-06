using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class NamePriorities
    {
        public int Id { get; set; }
        public string RegNumber { get; set; }
        public string Name { get; set; }
        public ChemicalType ChemicalType { get; set; }
    }

    public class NotificationTemplates
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Index("IX_NAME", IsUnique = true)]
        public string NotifactionName { get; set; }
        public string NotifactionMessage { get; set; }
    }
}
