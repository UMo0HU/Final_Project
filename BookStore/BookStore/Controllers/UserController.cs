using BookStore.Service.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IWishlistService _wishlistService;
        public UserController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        public async Task<IActionResult> Profile()
        {
            var wishlist = await _wishlistService.GetAllWishlists();
            return View(wishlist);
        }
    }
}
