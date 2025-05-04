using AspNetCoreGeneratedDocument;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace BookStore.Service.Cart
{
    public class CartService  : ICartService
    {
        private readonly BookStoreDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly ClaimsPrincipal _user;
        private readonly string _userId;
        private readonly bool _userCartExists;

        public CartService(BookStoreDBContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _user = _httpContextAccessor.HttpContext?.User;
            _userId = _userManager.GetUserId(_user);
            _userCartExists = _context.Carts.FirstOrDefault(c => c.UserId == _userId) != null? true : false;
        }

        public async Task<bool> AddBookToCart(int bookId)
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                if (!_userCartExists)
                {
                    await _context.Carts.AddAsync(new Models.Cart { UserId = _userId });
                    await _context.SaveChangesAsync();
                }

                var userCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == _userId);
                if (userCart != null)
                {
                    await _context.CartItems.AddAsync(new Models.CartItem { BookId = bookId, CartId = userCart.Id });
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }

            return false;
        }
        public async Task<bool> RemoveBookFromCart(int bookId)
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                if (!_userCartExists)
                {
                    await _context.Carts.AddAsync(new Models.Cart { UserId = _userId });
                    await _context.SaveChangesAsync();
                }

                var userCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == _userId);
                if (userCart != null)
                {
                    var cartItemToDelete = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == userCart.Id && ci.BookId == bookId);
                    if (cartItemToDelete != null)
                    {
                        _context.CartItems.Remove(cartItemToDelete);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }

            return false;
        }
        public async Task<bool> BookInCart(int bookId)
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                if (!_userCartExists)
                {
                    await _context.Carts.AddAsync(new Models.Cart { UserId = _userId });
                    await _context.SaveChangesAsync();
                }

                var userCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == _userId);
                if (userCart != null)
                {
                    var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == userCart.Id && ci.BookId == bookId);
                    if (cartItem != null)
                    {
                        return true;
                    }
                    return false;
                }

            }
            return false;
        }
        public async Task<List<Models.Book>> GetBooksFromCart()
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                if (!_userCartExists)
                {
                    await _context.Carts.AddAsync(new Models.Cart { UserId = _userId });
                    await _context.SaveChangesAsync();
                }

                var userCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == _userId);
                if (userCart != null)
                {
                    var books = _context.CartItems.Where(ci => ci.CartId == userCart.Id).Select(ci => ci.Book).ToList();
                    if (books != null)
                    {
                        return books;
                    }
                    return null;
                }

            }
            return null;
        }
    }
}
