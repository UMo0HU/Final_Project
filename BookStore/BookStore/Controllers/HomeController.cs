using System.Diagnostics;
using AspNetCoreGeneratedDocument;
using BookStore.Models;
using BookStore.Service.Book;
using BookStore.Service.Wishlist;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;
        private readonly IWishlistService _wishlistService;

        public HomeController(ILogger<HomeController> logger, IBookService bookService, IWishlistService wishlistService)
        {
            _logger = logger;
            _bookService = bookService;
            _wishlistService = wishlistService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await  _bookService.GetAllBooks();
            var Wishlist = await _wishlistService.GetUserWishlist();
            ViewBag.WishlistBookIds = Wishlist.Select(w =>  w.BookId).ToList();
            return View(books);
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
