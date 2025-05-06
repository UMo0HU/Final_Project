using BookStore.Models;
using BookStore.Service.Account;
using BookStore.Service.Cart;
using BookStore.Service.Wishlist;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.CodeDom;

namespace BookStore.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IWishlistService _wishlistService;
        private readonly ICartService _cartService;
        private readonly UserManager<User> _userManager;
        private readonly IAccountService _accountService;

        public UserController(IWishlistService wishlistService, ICartService cartService, UserManager<User> userManager, IAccountService accountService)
        {
            _wishlistService = wishlistService;
            _cartService = cartService;
            _userManager = userManager;
            _accountService = accountService;
        }
        public async Task<IActionResult> Profile()
        {
            var wishlist = await _wishlistService.GetUserWishlist();
            var books = wishlist.Select(w => w.Book).ToList();
            ViewBag.User = await _userManager.GetUserAsync(User);
            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> AccountManage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AccountManage(AccountManageViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var result = await _accountService.ChangeProfilePicture(model.ProfilePicture);
                    if(result)
                    {
                        model.ImageChanged = true;
                        return View(model);
                    }
                }
                catch(ArgumentException ex)
                {
                    ModelState.AddModelError(ex.ParamName, ex.Message.Split("(")[0]);
                }
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _accountService.ChangePassword(model.CurrentPassword, model.NewPassword);
                if(result)
                {
                    model.PasswordChanged = true;
                    return View(model);
                }
                ModelState.AddModelError(String.Empty, "Changing Password Failed");
                return View(model);
            }
            return View(model);
        }

        public async Task<IActionResult> CheckOut()
        {
            List<Book> books = await _cartService.GetBooksFromCart();
            
            return View(books);
        }
    }
}
