namespace BookStore.Service.Order
{
    public interface IOrderService
    {
        public Task<List<Models.Order>> GetAllOrders();
        public Task<Models.Order> GetOrderById(int id);

    }
}