using BookStore.Data;
using BookStore.Models;
using BookStore.Service.Book;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooks();
            return View(books);
        }
        public async Task<IActionResult> Details(int id)
        {
            Book book = await _bookService.GetBookDetails(id);
            return View(book);
        }
    }
}
