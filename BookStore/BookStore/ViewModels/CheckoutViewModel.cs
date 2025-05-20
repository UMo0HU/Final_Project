using BookStore.Models;
using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class CheckoutViewModel
    {
        public List<Book>? CartItems { get; set; }
        public Dictionary<int, int> Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        [Required]
        public string Address { get; set; }
        public string? PaymentIntentId { get; set; }
    }
}
