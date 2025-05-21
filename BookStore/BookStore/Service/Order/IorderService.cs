using BookStore.Models;
using BookStore.ViewModels;

namespace BookStore.Service.Order
{
    public interface IOrderService
    {
        public Task<List<Models.Order>> GetAllOrders();
        public Task<List<Models.Order>> GetUserOrders();
        public Task<Models.Order> GetOrderById(int id);
        public Task CreateOrder(CheckoutViewModel model);
        public Task<bool> RefundPayment(string paymentIntentId);
        public Task<bool> CancelOrder(int orderId);
        public Task<bool> ShipOrder(ShipmentViewModel model);
        public Task<bool> DeliverOrder(int orderId);
        public Task<Shipment> GetShipment(int orderId);
    }
}