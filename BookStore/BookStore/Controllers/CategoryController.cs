using BookStore.Service.Book;
using BookStore.Service.Category;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        public CategoryController(ICategoryService categoryService, IBookService bookService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Books = await _bookService.GetAllBooks();
            var categories = await _categoryService.GetAllCategories();
            return View(categories);
        }
    }
}
