using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Wishlist
{
    public class WishlistService : IWishlistService
    {
        private readonly BookStoreDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WishlistService(BookStoreDBContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Models.Wishlist>> GetAllWishlists()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(user);
                return await _context.Wishlists.Where(w => w.UserId == userId).Include(w => w.Book).ToListAsync();
            }
            return new List<Models.Wishlist>();
        }

    }
}
