using Newtonsoft.Json;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class TanData
    {
        public int Id { get; set; }
        [JsonIgnore]
        public virtual Tan Tan { get; set; }
        [ForeignKey("Tan")]
        [Index("IX_TAN", IsUnique = true)]
        public int TanId { get; set; }
        [Column(TypeName = "text")]
        public String Data { get; set; }
        public DateTime? Date { get; set; }
        public string User { get; set; }
        public string Ip { get; set; }
    }
}
