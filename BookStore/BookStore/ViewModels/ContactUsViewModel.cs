using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class ContactUsViewModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
        public bool SentSuccessfully = false;
    }
}
