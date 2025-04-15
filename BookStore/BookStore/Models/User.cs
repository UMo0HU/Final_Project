using Microsoft.AspNetCore.Identity;

namespace BookStore.Models
{
    public class User : IdentityUser
    {
        public byte[]? ProfilePicture { get; set; }

        public Cart Cart { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
