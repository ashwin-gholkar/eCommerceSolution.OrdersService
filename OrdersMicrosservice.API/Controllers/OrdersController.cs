using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace OrdersMicrosservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        [HttpGet("")]
        public async Task<IEnumerable<OrderResponse?>> Get()
        {
            List<OrderResponse?> orders =  await _orderService.GetOrders();
            return orders;
        }
        [HttpGet("search/orderid/{orderID}")]
        public async Task<OrderResponse?> GetOrderByOrderID(Guid orderID)
        {
            FilterDefinition<Order> filter = Builders<Order>.
                                            Filter.Eq(temp => temp.OrderID , orderID);
            OrderResponse? order = await _orderService.GetOrderByCondition(filter);
            return order;
        }
        [HttpGet("search/productid/{productID}")]
        public async Task<List<OrderResponse?>> GetOrderByProductID(Guid productID)
        {
            FilterDefinition<Order> filter = Builders<Order>.
                                            Filter.ElemMatch(temp => temp.OrderItems,
                                            Builders<OrderItem>.Filter.Eq(tempProduct
                                            => tempProduct.ProductID,productID  ));

            List<OrderResponse?> orders = await _orderService.GetOrdersByCondition(filter);
            return orders;
        }
        [HttpGet("search/orderDate/{orderDate}")]
        public async Task<List<OrderResponse?>> GetOrdersByOrderDate(DateTime orderDate)
        {
            FilterDefinition<Order> filter = Builders<Order>.
                                            Filter.Eq(temp => temp.OrderDate.ToString("yyy-MM-dd") ,
                                            orderDate.ToString("yyy-MM-dd"));

            List<OrderResponse?> orders = await _orderService.GetOrdersByCondition(filter);
            return orders;
        }
    }
}
