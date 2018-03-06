using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductTracking.Models
{
    public class NamePrioritiesViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string RegNumber { get; set; }
        [Required]
        public string oper { get; set; }
        [Required]
        public ChemicalType ChemicalType { get; set; }
    }
}