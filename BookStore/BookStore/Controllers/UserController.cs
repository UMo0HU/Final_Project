using BookStore.Models;
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
        private readonly UserManager<User> _userManager;

        public UserController(IWishlistService wishlistService, UserManager<User> userManager)
        {
            _wishlistService = wishlistService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Profile()
        {
            var wishlist = await _wishlistService.GetUserWishlist();
            var books = wishlist.Select(w => w.Book).ToList();
            ViewBag.User = await _userManager.GetUserAsync(User);
            return View(books);
        }

    }
}
