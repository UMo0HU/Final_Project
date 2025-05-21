using BookStore.Service.Book;
using BookStore.Service.Cart;
using BookStore.Service.Category;
using BookStore.Service.Wishlist;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace BookStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        private readonly IWishlistService _wishlistService;
        private readonly ICartService _cartService;
        public CategoryController(ICategoryService categoryService, IBookService bookService, IWishlistService wishlistService, ICartService cartService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
            _wishlistService = wishlistService;
            _cartService = cartService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var wishlist = await _wishlistService.GetUserWishlist();
            var cart = await _cartService.GetBooksFromCart();
            var categories = await _categoryService.GetAllCategories();

            ViewBag.Books = await _bookService.GetAllBooks();
            ViewBag.WishlistBookIds = wishlist.Select(w => w.BookId).ToList();
            ViewBag.CartBookIds = cart.Select(b => b.Id).ToList();
            return View(categories);
        }

        public async Task<IActionResult> GetBooksByCategory(int categoryId)
        {
            var wishlistBookIds = (await _wishlistService.GetUserWishlist()).Select(w => w.BookId).ToList();
            var cartBookIds = (await _cartService.GetBooksFromCart()).Select(b => b.Id).ToList();
            var books = await _bookService.GetBooksForCategory(categoryId);
            
            var model = new ShowBooksViewModel
            {
                books = books,
                UserWishlist = wishlistBookIds,
                UserCart = cartBookIds
            };

            return PartialView("_LoadBooksPartial", model);
        }
    }
}
