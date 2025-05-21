using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Email Is Required.")]
        [EmailAddress]
        public string Email { get; set; }

        public bool EmailSent { get; set; }

    }
}
