using BookStore.Service.Order;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using BookStore.Models;
using System.Security.Claims;

public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [HttpGet]
    public async Task<IActionResult> Track(int id)
    {
        var order = await _orderService.GetOrderById(id);
        if (order == null)
        {
            ViewBag.Message = "Order not found";
            return View();
        }


        return View(order);

    }
    [HttpGet("/order/orderList")]
    public async Task<IActionResult> OrderList()
    {
        var orders = await _orderService.GetAllOrders();

        if (orders == null || !orders.Any())
        {
            ViewBag.Message = "No orders found.";
            return View(new List<Order>());
        }

        return View(orders);
    }
}
