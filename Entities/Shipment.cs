using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core
{
    public class Batch
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public DateTime? DateCreated { get; set; }
        public string DocumentsPath { get; set; }
        public string GifImagesPath { get; set; }
    }

    
}