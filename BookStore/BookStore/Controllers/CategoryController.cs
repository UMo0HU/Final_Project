using BookStore.Service.Book;
using BookStore.Service.Category;
using BookStore.Service.Wishlist;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        private readonly IWishlistService _wishlistService;
        public CategoryController(ICategoryService categoryService, IBookService bookService, IWishlistService wishlistService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
            _wishlistService = wishlistService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Books = await _bookService.GetAllBooks();
            var Wishlist = await _wishlistService.GetUserWishlist();
            ViewBag.WishlistBookIds = Wishlist.Select(w => w.BookId).ToList();
            var categories = await _categoryService.GetAllCategories();
            return View(categories);
        }
    }
}
