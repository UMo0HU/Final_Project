using BookStore.Data;
using BookStore.Models;
using BookStore.Service.Book;
using BookStore.Service.Wishlist;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IWishlistService _wishlistService;

        public BookController(IBookService bookService, IWishlistService wishlistService)
        {
            _bookService = bookService;
            _wishlistService = wishlistService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooks();
            return View(books);
        }
        public async Task<IActionResult> Details(int id)
        {
            Book book = await _bookService.GetBookDetails(id);
            ViewBag.BookCategories = book.Book_Categories
                .Select(bc => bc.Category)
                .ToList();
            ViewBag.BookInWishlist = await _wishlistService.IsBookInWishlist(id);
            return View(book);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWishlist(int id)
        {
            var result = await _wishlistService.AddToWishlist(id);
            return Json(new {success = result});
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            var result = await _wishlistService.RemoveFromWishlist(id);
            return Json(new { success = result });
        }
    }
}
