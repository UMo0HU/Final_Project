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
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Options;
using Stripe.Tax;
using System.Runtime.CompilerServices;

namespace BookStore.Controllers
{
    [IgnoreAntiforgeryToken]
    public class PaymentController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly StripeSettings _stripeSettings;


        public PaymentController(IWishlistService wishlistService, ICartService cartService, UserManager<User> userManager, IAccountService accountService, IOrderService orderService, IOptions<StripeSettings> stripeSettings)
        {
            _cartService = cartService;
            _orderService = orderService;
            _stripeSettings = stripeSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            CheckoutViewModel model = new CheckoutViewModel();

            List<Book> cartItems = await _cartService.GetBooksFromCart();
            model.CartItems = cartItems;

            return View(model);
        }

        public async Task<IActionResult> Payment(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cartItems = await _cartService.GetBooksFromCart();
                var currency = "usd";
                var successUrl = $"{Request.Scheme}://{Request.Host}/Payment/Success";
                var cancelUrl = $"{Request.Scheme}://{Request.Host}/Payment/Cancel";
                StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = successUrl,
                    CancelUrl = cancelUrl,
                };

                foreach (var item in cartItems)
                {
                    var sessionListItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmountDecimal = item.Price * 100,
                            Currency = currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Title,
                                Description = item.Description,
                            },
                        },
                        Quantity = model.Quantity[item.Id],
                    };
                    options.LineItems.Add(sessionListItem);
                }
                var service = new SessionService();
                var session = await service.CreateAsync(options);

                TempData["sessionId"] = session.Id;
                TempData["checkoutModel"] = TempDataHelper.GetObjectString(model);
                TempData.Keep();

                return Redirect(session.Url);
            }
            return View("CheckOut");
        }

        public async Task<IActionResult> Success()
        {
            var checkoutData = TempData["checkoutModel"] as string;
            var sessionId = TempData["sessionId"] as string;
            if (string.IsNullOrEmpty(checkoutData) || string.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("CheckOut");
            }

            var checkoutModel = TempDataHelper.GetObject<CheckoutViewModel>(checkoutData);
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                checkoutModel.PaymentIntentId = session.PaymentIntentId;
                await _orderService.CreateOrder(checkoutModel);
                return View();
            }
            return RedirectToAction("CheckOut");
        }

        public IActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var result = await _orderService.CancelOrder(orderId);
            if (result)
                return Json(new { success = result });

            return Json(new
            {
                success = false,
                errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> OrderDelivered(int orderId)
        {
            var result = await _orderService.DeliverOrder(orderId);
            if (result)
                return Json(new { success = result });
            return Json(new
            {
                success = false,
                errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList()
            });
        }


    }
}
