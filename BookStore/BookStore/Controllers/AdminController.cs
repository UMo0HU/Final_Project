using BookStore.Models;
using BookStore.Service.Book;
using BookStore.Service.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        public AdminController(ICategoryService categoryService, IBookService bookService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = categories;
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Categories(Category category)
        {
            var categories = await _categoryService.GetAllCategories();
            if (ModelState.IsValid)
            {
                await _categoryService.AddCategory(category);
                return RedirectToAction("Categories");
            }
            ViewBag.Categories = categories;
            return View(category);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategory(id);
            return RedirectToAction("Categories");
        }
        [HttpGet]
        public async Task<IActionResult> Books()
        {
            var books = await _bookService.GetAllBooks();
            ViewBag.Books = books;
            return View(new Book());
        }

        [HttpPost]
        public async Task<IActionResult> Books(Book book)
        {
            var books = await _bookService.GetAllBooks();
            if (ModelState.IsValid)
            {
                await _bookService.AddBook(book);
                return RedirectToAction("Books");
            }
            ViewBag.Books = books;
            return View(book);
        }
    }
}
