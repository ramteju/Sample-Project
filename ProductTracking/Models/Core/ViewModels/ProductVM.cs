using System;
using System.ComponentModel.DataAnnotations;

namespace ProductTracking.Models.Core.ViewModels
{
    public class CalendarVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Target { get; set; }
        public bool IsDeliveryScheduled { get; set; }

        [Required]
        public string oper { get; set; }
    }

    public class ShipmentVM
    {
        [Required(ErrorMessage = "Name is mandatory")]
        public int Name { get; set; }
        public DateTime ShipmentDate { get; set; }
        public DateTime? DelivaryDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int productId { get; set; }
        public string ShipmentPath { get; set; }

        [Required]
        public string oper { get; set; }
    }
}