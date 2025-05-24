using BookStore.Models;
using BookStore.Service.Book;
using BookStore.Service.Category;
using BookStore.Service.Order;
using BookStore.Service.User;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        public AdminController(ICategoryService categoryService, IBookService bookService, IOrderService orderService, IUserService userService, UserManager<User> userManager)
        {
            _categoryService = categoryService;
            _bookService = bookService;
            _orderService = orderService;
            _userService = userService;
            _userManager = userManager;
        }
        // Admin Dashboard:
        public async Task<IActionResult> Index()
        {

            var totalBooks = (await _bookService.GetAllBooks()).Count();
            var totalCategories = (await _categoryService.GetAllCategories()).Count();
            var totalOrders = (await _orderService.GetAllOrders()).Count();
            var sales = (await _orderService.GetAllOrders()).Where(o => o.Status.ToLowerInvariant() != "canceled").Sum(o => o.TotalAmount);
            var users = (await _userService.GetAllUsers()).Count();
            var pendingOrders = (await _orderService.GetAllOrders()).Where(o => o.Status.ToLowerInvariant() == "pending").Count();
            var canceledOrders = (await _orderService.GetAllOrders()).Where(o => o.Status.ToLowerInvariant() == "canceled").Count();
            var shippedOrders = (await _orderService.GetAllOrders()).Where(o => o.Status.ToLowerInvariant() == "shipped").Count();
            var deliveredOrders = (await _orderService.GetAllOrders()).Where(o => o.Status.ToLowerInvariant() == "delivered").Count();

            var dashboardModel = new DashboardViewModel()
            {
                TotalBooks = totalBooks,
                TotalCategories = totalCategories,
                Users = users,
                Sales = sales,
                TotalOrders = totalOrders,
                CanceledOrders = canceledOrders,
                DeliveredOrders = deliveredOrders,
                PendingOrders = pendingOrders,
                SippedOrders = shippedOrders
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
        public async Task<IActionResult> EditBook(int id, BookViewModel model)
        {
            model.Id = id;
            if (ModelState.IsValid)
            {
                await _bookService.EditBook(model);
                return RedirectToAction("Books");
            }
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

        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Shipment(int orderId)
        {
            var shipmentViewModel = new ShipmentViewModel();
            string trackingNumber = "TRK" + DateTime.Now.ToString("yyyyMMddhhmmss");
            shipmentViewModel.TrackingNumber = trackingNumber;
            shipmentViewModel.OrderId = orderId;
            var shipment = await _orderService.GetShipment(orderId);
            shipmentViewModel.Address = shipment.ShippingAddress;
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

        // User Management:
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsers();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> BanUser(string userId)
        {
            var model = new BanUserViewModel();
            model.UserId = userId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(BanUserViewModel model)
        {
            var result = await _userService.BanUser(model.UserId, model.BanEndDate);
            if (result)
            {
                TempData["BanUser"] = result;
                return RedirectToAction("Users", "Admin");
            }
            return RedirectToAction("Users", "Admin");
        }

        public async Task<IActionResult> UnBanUser(string userId)
        {
            var result = await _userService.UnBanUser(userId);
            if (result)
            {
                TempData["UnBanUser"] = result;
                return RedirectToAction("Users", "Admin");
            }
            return RedirectToAction("Users", "Admin");
        }
    }
}
