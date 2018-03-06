using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SolventBoilingPoints
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        [Index("IX_REG", IsUnique = true)]
        public string RegNo { get; set; }
        public string Name { get; set; }
        public float DegreesBoilingPoint { get; set; }
        public float KelvinBoilingPoint { get; set; }
        public float RankineBoilingPoint { get; set; }
        public float fahrenheitBoilingPoint { get; set; }
    }
}
