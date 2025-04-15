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
        public IActionResult Index()
        {
            var books = _bookService.GetAllBooks();
            return View(books);
        }
        public IActionResult Details(int id)
        {
            Book book = _bookService.GetBookDetails(id);
            return View(book);
        }
    }
}
