using BookStore.Data;
using BookStore.Helper;
using BookStore.Models;
using BookStore.Service.Account;
using BookStore.Service.Cart;
using BookStore.Service.Order;
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
        private readonly UserManager<User> _userManager;
        private readonly IAccountService _accountService;
        private readonly ICartService _cartService;

        public UserController(IWishlistService wishlistService, ICartService cartService, UserManager<User> userManager, IAccountService accountService, IOrderService orderService)
        {
            _wishlistService = wishlistService;
            _userManager = userManager;
            _accountService = accountService;
            _cartService = cartService;
        }
        public async Task<IActionResult> Profile()
        {
            var wishlist = await _wishlistService.GetUserWishlist();
            var books = wishlist.Select(w => w.Book).ToList();
            var cart = await _cartService.GetBooksFromCart();

            ViewBag.WishlistBookIds = wishlist.Select(w => w.BookId).ToList();
            ViewBag.CartBookIds = cart.Select(b => b.Id).ToList();
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
                    var imageChange = await _accountService.ChangeProfilePicture(model.ProfilePicture);
                    var usernameChange = await _accountService.ChangeUsername(model.Username);
                    if(imageChange)
                    {
                        model.ImageChanged = true;
                        return View(model);
                    }
                    else if(usernameChange)
                    {
                        model.UsernameChanged = true;
                        return View(model);
                    }
                    else if(imageChange && usernameChange)
                    {
                        model.ImageChanged = true;
                        model.UsernameChanged = true;
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
    }
}
