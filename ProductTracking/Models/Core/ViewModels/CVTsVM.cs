using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductTracking.Models.Core.ViewModels
{
    public class DelivarableCVTsVM
    {
        [Required(ErrorMessage = "CVT is mandatory")]
        public string CVT { get; set; }
        public string AssociatedFreetext { get; set; }
        [Required]
        public string oper { get; set; }
    }

    public class DelivarablefreeTextVM
    {
        [Required(ErrorMessage = "Freetext is mandatory")]
        public string Freetext { get; set; }
        [Required]
        public string oper { get; set; }
    }

    public class IgnorableCVTsVM
    {
        [Required(ErrorMessage = "CVT is mandatory")]
        public string CVT { get; set; }
        [Required(ErrorMessage = "AssociatedFreetext is mandatory")]
        public string AssociatedFreetext { get; set; }
        [Required(ErrorMessage = "ExistingType is mandatory")]
        public ParticipantType ExistingType { get; set; }
        [Required(ErrorMessage = "New type is mandatory")]
        public ParticipantType NewType { get; set; }
        [Required]
        public string oper { get; set; }
    }
}