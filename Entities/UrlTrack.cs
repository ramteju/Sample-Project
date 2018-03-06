using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core
{
    public class UrlTrack
    {
        public int Id { get; set; }
        public string Url { get; set; }
        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public DateTime TimeStamp { get; set; }

        public UrlTrack()
        {
            TimeStamp = DateTime.Now;
        }
    }
}