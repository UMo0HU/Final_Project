using BookStore.Data;
using BookStore.Helper;
using BookStore.Models;
using BookStore.Service.Account;
using BookStore.Service.Cart;
using BookStore.Service.Order;
using BookStore.Service.Wishlist;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly PaypalClient _paypalClient;

        public PaymentController(IWishlistService wishlistService, ICartService cartService, UserManager<User> userManager, IAccountService accountService, IOrderService orderService, PaypalClient paypalClient)
        {
            _cartService = cartService;
            _orderService = orderService;
            _paypalClient = paypalClient;
        }

        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            CheckoutViewModel model = new CheckoutViewModel();

            List<Book> cartItems = await _cartService.GetBooksFromCart();
            model.CartItems = cartItems;


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (SessionHelper.GetObjectFromJson<CheckoutViewModel>(HttpContext.Session, "cart") == null)
                {
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", model);
                }
                else
                {
                    CheckoutViewModel cart = SessionHelper.GetObjectFromJson<CheckoutViewModel>(HttpContext.Session, "cart");
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                }
                return RedirectToAction("Payment");
            }

            return View(model);
        }


        public async Task<IActionResult> Payment()
        {
            if (SessionHelper.GetObjectFromJson<CheckoutViewModel>(HttpContext.Session, "cart") != null)
            {
                CheckoutViewModel cart = SessionHelper.GetObjectFromJson<CheckoutViewModel>(HttpContext.Session, "cart");
                ViewBag.TotalAmount = cart.TotalAmount;
                return View();
            }
            return RedirectToAction("Checkout", "User");
        }

        [HttpPost]
        public async Task<IActionResult> Order(CancellationToken cancellationToken)
        {
            try
            {
                CheckoutViewModel cart = SessionHelper.GetObjectFromJson<CheckoutViewModel>(HttpContext.Session, "cart");

                var price = $"{cart.TotalAmount}";
                var currency = "USD";

                var reference = GetRandomInvoiceNumber();

                HttpContext.Session.SetString("paypal_reference", reference);

                var response = await _paypalClient.CreateOrder(price, currency, reference);

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }
        public async Task<IActionResult> Capture(string orderId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                var reference = HttpContext.Session.GetString("paypal_reference");

                HttpContext.Session.Remove("paypal_reference");

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

        public static string GetRandomInvoiceNumber()
        {
            return new Random().Next(999999).ToString();
        }

        public async Task<IActionResult> Success()
        {
            CheckoutViewModel cart = SessionHelper.GetObjectFromJson<CheckoutViewModel>(HttpContext.Session, "cart");
            await _orderService.CreateOrder(cart);
            SessionHelper.RemoveObject(HttpContext.Session, "cart");
            return View();
        }
    }
}
