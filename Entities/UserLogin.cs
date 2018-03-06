using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core
{
    public class UserLogin
    {
        public int Id { get; set; }
        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Ip { get; set; }

        public UserLogin()
        {
            Timestamp = DateTime.Now;
        }
    }
}