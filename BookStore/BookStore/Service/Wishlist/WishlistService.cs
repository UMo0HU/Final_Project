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

        public async Task<List<Models.Wishlist>> GetUserWishlist()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(user);
                return await _context.Wishlists.Where(w => w.UserId == userId).Include(w => w.Book).Include(w => w.User).ToListAsync();
            }
            return new List<Models.Wishlist>();
        }

        public async Task<bool> IsBookInWishlist(int bookId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(user);
                return await _context.Wishlists.AnyAsync(w => w.BookId == bookId && w.UserId == userId);
            }
            return false;
        }
        public async Task<bool> AddToWishlist(int bookId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {


                var userId = _userManager.GetUserId(user);
                var wishlistItem = new Models.Wishlist
                {
                    BookId = bookId,
                    UserId = userId
                };


                await _context.Wishlists.AddAsync(wishlistItem);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> RemoveFromWishlist(int bookId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(user);
                var wishlistItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.BookId == bookId && w.UserId == userId);
                if (wishlistItem != null)
                {
                    _context.Wishlists.Remove(wishlistItem);
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }
    }
}
