using System.ComponentModel.DataAnnotations;

namespace BookStore.Helper
{
    public enum Carrier
    {
        FedEx,
        Aramex,

        [Display(Name = "J&T Express")]
        JAndTExpress
    }
}
