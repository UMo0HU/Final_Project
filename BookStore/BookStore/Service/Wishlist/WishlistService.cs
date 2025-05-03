using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.Service.Wishlist
{
    public class WishlistService : IWishlistService
    {
        private readonly BookStoreDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public WishlistService(BookStoreDBContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext?.User;
        }

        public async Task<List<Models.Wishlist>> GetUserWishlist()
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(_user);
                return await _context.Wishlists.Where(w => w.UserId == userId).Include(w => w.Book).Include(w => w.User).ToListAsync();
            }
            return new List<Models.Wishlist>();
        }

        public async Task<bool> IsBookInWishlist(int bookId)
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(_user);
                return await _context.Wishlists.AnyAsync(w => w.BookId == bookId && w.UserId == userId);
            }
            return false;
        }
        public async Task<bool> AddToWishlist(int bookId)
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {


                var userId = _userManager.GetUserId(_user);
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
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(_user);
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
