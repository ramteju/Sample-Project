using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductTracking.Models.Core
{
    public class FtpFolder
    {
        public int Id { get; set; }
        [Index("IX_PATH", IsUnique = true)]
        [MaxLength(500)]
        public string Path { get; set; }

        public int DaysToSubmit { get; set; }
    }


}