using BookStore.Helper;
using Google.Apis.Util;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BookStore.ViewModels
{
    public class ShipmentViewModel
    {
        public int OrderId { get; set; }
        public string? Address { get; set; }
        [Required(ErrorMessage = "Tracking number is required")]
        public string TrackingNumber { get; set; }
        [Required(ErrorMessage = "Please select a carrier")]
        [Display(Name = "Shipping Carrier")]
        public Carrier Carrier { get; set; }
    }
}
