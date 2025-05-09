using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.Service.Order
{
    public class OrderService : IOrderService
    {
        private readonly BookStoreDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly UserManager<User> _userManager;

        public OrderService(BookStoreDBContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext?.User;
        }

        public async Task<List<Models.Order>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .ToListAsync();
        }
        public async Task<Models.Order> GetOrderById(int id)
        {
            return await _context.Orders
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
                    Payment = new Models.Payment
                    {
                        Amount = model.TotalAmount,
                        PaymentDate = DateTime.UtcNow,
                        PaymentMethod = "Paypal",
                        Status = "Paid"
                    },
                    Shipment = new Shipment
                    {
                        Carrier = "FedEx",
                        Status = "Shipped",
                        ShipmentDate = DateTime.UtcNow,
                        ShippingAddress = model.Address,
                        TrackingNumber = "123456"
                    },
                    UserId = user.Id,
                    Status = "Shipped",
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

    }

}