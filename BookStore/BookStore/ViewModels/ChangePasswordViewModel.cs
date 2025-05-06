using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword",ErrorMessage = "New Password & Confirm New Password Are Not The Same.")]
        public string ConfirmNewPassword { get; set; }

        public bool PasswordChanged = false;
    }
}
