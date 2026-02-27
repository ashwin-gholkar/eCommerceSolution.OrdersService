using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Data;

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
        [HttpPost]
        public async Task<IActionResult?> AddOrder(OrderAddRequest orderAddRequest)
        {
            if (orderAddRequest == null) return BadRequest("Invalid order data");
            OrderResponse? orderResponse = await _orderService.AddOrder(orderAddRequest);
            if (orderResponse == null) return Problem("Error in adding product");
            return Created($"api/orders/search/orderid/{orderResponse?.OrderID}", orderResponse);
        }
        [HttpPut]
        public async Task<IActionResult?> OrderUpdate(Guid orderID, OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest == null) return BadRequest("Invalid order data");
            if (orderID != orderUpdateRequest.OrderID) return BadRequest("OrderID in the URL doesnt match with the OrderID in the request body");


            OrderResponse? orderResponse = await _orderService.UpdateOrder(orderUpdateRequest);
            if (orderResponse == null) return Problem("Error in adding product");
            return Ok(orderResponse);
        }

        [HttpDelete]
        public async Task<IActionResult?> OrderUpdate(Guid orderID)
        {
            if (orderID == Guid.Empty) return BadRequest("Invalid OrderID");


            bool isDeleted = await _orderService.DeleteOrder(orderID);
            if (!isDeleted) return Problem("Error in deleting product");
            return Ok(isDeleted);
        }
        [HttpGet("search/userid/{userID}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrdersByOrderDate(Guid userID)
        {
            FilterDefinition<Order> filter = Builders<Order>.
                                            Filter.Eq(temp => temp.UserID,userID);

            List<OrderResponse?> orders = await _orderService.GetOrdersByCondition(filter);
            return orders;
        }
    }
}
