using BookStore.Models;
using BookStore.Service.Cart;
using BookStore.Service.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;

namespace BookStore.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IWishlistService _wishlistService;
        private readonly ICartService _cartService;
        private readonly UserManager<User> _userManager;

        public UserController(IWishlistService wishlistService, ICartService cartService, UserManager<User> userManager)
        {
            _wishlistService = wishlistService;
            _cartService = cartService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Profile()
        {
            var wishlist = await _wishlistService.GetUserWishlist();
            var books = wishlist.Select(w => w.Book).ToList();
            ViewBag.User = await _userManager.GetUserAsync(User);
            return View(books);
        }

        public async Task<IActionResult> CheckOut()
        {
            List<Book> books = await _cartService.GetBooksFromCart();
            
            return View(books);
        }
    }
}
