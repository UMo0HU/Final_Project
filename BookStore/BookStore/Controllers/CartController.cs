using BookStore.Service.Cart;
using Google.Apis.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> AddToCart(int bookId)
        {
            var result = await _cartService.AddBookToCart(bookId);

            return Json(new { success = result });
        }

        public async Task<IActionResult> RemoveFromCart(int bookId)
        {
            var result = await _cartService.RemoveBookFromCart(bookId);

            return Json(new { success = result });
        }

    }
}
