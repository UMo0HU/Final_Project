using BookStore.Models;
using BookStore.Service.Book;
using BookStore.Service.Category;
using BookStore.Service.Order;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;
        public AdminController(ICategoryService categoryService, IBookService bookService, IOrderService orderService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
            _orderService = orderService;
        }
        // Admin Dashboard:
        public async Task<IActionResult> Index()
        {

            var totalBooks = (await _bookService.GetAllBooks()).Count();
            var totalOrders = (await _orderService.GetAllOrders()).Count();
            var sales = (await _orderService.GetAllOrders()).Where(o => o.Status.ToLowerInvariant() != "canceled").Sum(o => o.TotalAmount);

            var dashboardModel = new DashboardViewModel()
            {
                TotalBooks = totalBooks,
                TotalOrders = totalOrders,
                Sales = sales
            };
            return View(dashboardModel);
        }

        // Categories Pages:
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

        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategory(id);
            return RedirectToAction("Categories");
        }

        // Books Pages:
        [HttpGet]
        public async Task<IActionResult> Books()
        {
            var bookViewModel = new BookViewModel();
            var books = await _bookService.GetAllBooks();
            var categories = await _categoryService.GetAllCategories();
            ViewBag.Books = books;
            return View(bookViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _bookService.GetBookDetails(id);
            var categories = await _categoryService.GetAllCategories();
            var bookCategory = book.Book_Categories.Select(bc => bc.CategoryId).ToList();
            var bookViewModel = new BookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Price = book.Price,
                Stock = book.Stock,
                AllCategories = categories,
                SelectedCategoryIds = bookCategory
            };
            ViewBag.BookImg = book.Img;
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

        [HttpGet]
        public async Task<IActionResult> AddBook()
        {
            var bookViewModel = new BookViewModel();
            var books = await _bookService.GetAllBooks();
            var categories = await _categoryService.GetAllCategories();
            bookViewModel.AllCategories = categories;
            ViewBag.Books = books;
            return View(bookViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookViewModel bookViewModel)
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

        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteBook(id);
            return RedirectToAction("Books");
        }

        //Orders Pages:
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderService.GetAllOrders();
            return View(orders);
        }

        [HttpGet]
        public IActionResult Shipment(int orderId)
        {
            var shipmentViewModel = new ShipmentViewModel();
            string trackingNumber = "TRK" + DateTime.Now.ToString("yyyyMMddhhmmss");
            shipmentViewModel.TrackingNumber = trackingNumber;
            shipmentViewModel.OrderId = orderId;
            return View(shipmentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Shipment(ShipmentViewModel model)
        {
            if(ModelState.IsValid)
            {
               var result = await _orderService.ShipOrder(model);
                if (result)
                {
                    TempData["Shipment"] = result;
                    return RedirectToAction("Orders", "Admin");
                }
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var orders = (await _orderService.GetAllOrders()).Where(o => o.Status.ToLowerInvariant() == status.ToLowerInvariant()).ToList();

            return PartialView("_OrdersPartial", orders);
        }
    }
}
