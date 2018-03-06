using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core
{
    public class SessionResult
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsSuccess { get; set; }
        public string Exception { get; set; }
        public int TansCount { get; set; }
    }
}