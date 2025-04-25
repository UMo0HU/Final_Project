using BookStore.Models;
using BookStore.Service.Book;
using BookStore.Service.Category;
using BookStore.ViewModels;
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

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category updatedCategory)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.EditCategory(updatedCategory);
                return RedirectToAction("Categories");
            }
            return View(updatedCategory);
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
            var bookViewModel = new BookViewModel();
            var books = await _bookService.GetAllBooks();
            var categories = await _categoryService.GetAllCategories();
            bookViewModel.AllCategories = categories;
            ViewBag.Books = books;
            return View(bookViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Books(BookViewModel bookViewModel)
        {
            var books = await _bookService.GetAllBooks();
            var categories = await _categoryService.GetAllCategories();
            if (ModelState.IsValid)
            {
                await _bookService.AddBook(bookViewModel);
                return RedirectToAction("Books");
            }
            ViewBag.Books = books;
            bookViewModel.AllCategories = categories;
            return View(bookViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _bookService.GetBookDetails(id);
            var categories = await _categoryService.GetAllCategories();
            var bookViewModel = new BookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Price = book.Price,
                Stock = book.Stock,
                AllCategories = categories,
            };
            return View(bookViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(int id, BookViewModel bookViewModel)
        {
            bookViewModel.Id = id;
            if (ModelState.IsValid)
            {
                await _bookService.EditBook(bookViewModel);
                return RedirectToAction("Books");
            }
            return View(bookViewModel);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteBook(id);
            return RedirectToAction("Books");
        }
    }
}
