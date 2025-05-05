using BookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Order
{
    public class OrderService : IOrderService
    {
        private readonly BookStoreDBContext _context;

        public OrderService(BookStoreDBContext context)
        {
            _context = context;
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


    }

}