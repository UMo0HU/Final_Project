using System.Diagnostics;
using AspNetCoreGeneratedDocument;
using BookStore.Models;
using BookStore.Service.Book;
using BookStore.Service.Email;
using BookStore.Service.Wishlist;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;
        private readonly IWishlistService _wishlistService;
        private readonly IEmailSenderService _emailSenderService;

        public HomeController(ILogger<HomeController> logger, IBookService bookService, IWishlistService wishlistService, IEmailSenderService emailSenderService, UserManager<User> userManager)
        {
            _logger = logger;
            _bookService = bookService;
            _wishlistService = wishlistService;
            _emailSenderService = emailSenderService;
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
            var model = new ContactUsViewModel();
            if (TempData["SentSuccessfully"] != null)
            {
                model.SentSuccessfully = true;
            }

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactUsViewModel model)
        {
            if(ModelState.IsValid)
            {
                await _emailSenderService.ContactUsAsync(model.Subject, model.Message);
                TempData["SentSuccessfully"] = true;

                return RedirectToAction("ContactUs");
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
