using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class BanUserViewModel
    {
        public string UserId { get; set; }
        [Display(Name = "Ban User")]
        public DateTime BanEndDate { get; set; }

    }
}
