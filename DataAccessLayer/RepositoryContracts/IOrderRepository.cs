using DataAccessLayer.Entities;
using MongoDB.Driver;

namespace DataAccessLayer.RepositoryContracts
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByCondition(FilterDefinition<Order> filter);
        Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Order?> AddOrder(Order order);
        Task<Order?> UpdateOrder(Order order);
        Task<bool> DeleteOrder(Guid orderID);
    }
}
