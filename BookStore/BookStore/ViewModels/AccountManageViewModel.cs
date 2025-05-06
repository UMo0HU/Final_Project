using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class AccountManageViewModel
    {
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { set; get; }

        public bool ImageChanged = false;
    }
}
