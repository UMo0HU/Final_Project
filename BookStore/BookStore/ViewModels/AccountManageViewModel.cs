using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class AccountManageViewModel
    {
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9_-]+$", ErrorMessage = "Must start with a letter, Then use letters, numbers, dashes (-), or underscores (_).")]
        [Display(Name = "Username")]
        public string? Username { get; set; }
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { set; get; }

        public bool ImageChanged = false;

        public bool UsernameChanged = false;
    }
}
