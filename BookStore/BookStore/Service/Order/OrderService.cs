using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using System.Security.Claims;

namespace BookStore.Service.Order
{
    public class OrderService : IOrderService
    {
        private readonly BookStoreDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly UserManager<Models.User> _userManager;
        private readonly StripeSettings _stripeSettings;

        public OrderService(BookStoreDBContext context, IHttpContextAccessor httpContextAccessor, UserManager<Models.User> userManager, IOptions<StripeSettings> stripeSettings)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext?.User;
            _stripeSettings = stripeSettings.Value;
        }

        public async Task<List<Models.Order>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();
        }

        public async Task<List<Models.Order>> GetUserOrders()
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(_user);
                return await _context.Orders
                    .Where(o => o.UserId == user.Id)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .ToListAsync();
            }
            return new List<Models.Order>();
        }

        public async Task<Models.Order> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.Shipment)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task CreateOrder(CheckoutViewModel model)
        {
            if(_user != null && _user.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(_user);
                //Created Order Object To Add It To DB:
                var order = new Models.Order()
                {
                    OrderDate = DateTime.UtcNow,
                    Payment = new Payment
                    {
                        Amount = model.TotalAmount,
                        PaymentDate = DateTime.UtcNow,
                        PaymentMethod = "Stripe",
                        Status = "PAID",
                        PaymentIntentId = model.PaymentIntentId
                    },
                    Shipment = new Shipment
                    {
                        Status = "PENDING",
                        ShippingAddress = model.Address, 
                    },
                    UserId = user.Id,
                    Status = "PENDING",
                    TotalAmount = model.TotalAmount,
                    OrderItems = new List<OrderItem>()
                };

                await _context.Orders.AddAsync(order);

                await _context.SaveChangesAsync();

                int orderId = order.Id;

                var userCart = await _context.CartItems.ToListAsync();

                foreach(var item in model.Quantity) {
                    int bookId = item.Key;
                    int quantity = item.Value;

                    var book = await _context.Books.FindAsync(bookId);
                    if (book == null) continue;

                    //Update Book Stock:
                    book.Stock -= quantity;
                    _context.Books.Update(book);

                    //Add Order:
                    order.OrderItems.Add(new OrderItem
                    {
                        BookId = bookId,
                        Quantity = quantity,
                        OrderId = orderId
                    });
                }

                await _context.SaveChangesAsync();

                var userCartItems = await _context.CartItems
                .Where(c => c.Cart.UserId == user.Id)
                .ToListAsync();

                _context.CartItems.RemoveRange(userCartItems);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<bool> RefundPayment(string paymentIntentId)
        {
            try
            {
                StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                };
                var refundService = new RefundService();
                Refund refund = await refundService.CreateAsync(refundOptions);

                return refund.Status == "succeeded";
            }
            catch(StripeException ex)
            {
                return false;
            }
        }
        public async Task<bool> CancelOrder(int orderId)
        {
            var order = await _context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null && order.Payment != null &&
                order.Status.ToLowerInvariant() != "canceled" &&
                order.Status.ToLowerInvariant() != "delivered")
            {
                var refunded = await RefundPayment(order.Payment.PaymentIntentId);
                if (refunded)
                {
                    order.Status = "CANCELED";
                    order.Payment.Status = "REFUNDED";
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ShipOrder(ShipmentViewModel model) 
        {
            var order = await _context.Orders
                .Include(o => o.Shipment)
                .FirstOrDefaultAsync(o => o.Id == model.OrderId);
            if (order != null && order.Status.ToLowerInvariant() != "canceled" && order.Status.ToLowerInvariant() != "delivered")
            {
                order.Shipment.TrackingNumber = model.TrackingNumber;
                order.Shipment.Carrier = model.Carrier.ToString();
                order.Shipment.Status = "SHIPPED";
                order.Status = "SHIPPED";
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeliverOrder(int orderId) 
        {
            var order = await _context.Orders
                .Include(o => o.Shipment)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null && order.Status.ToLowerInvariant() != "canceled" && order.Status.ToLowerInvariant() != "delivered")
            {
                order.Shipment.Status = "DELIVERED";
                order.Status = "DELIVERED";
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Shipment> GetShipment(int orderId) 
        {
            var order = await _context.Orders
                .Include(o => o.Shipment)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null && order.Shipment != null)
            {
                return order.Shipment;
            }
            return new Shipment();
        }
    }

}