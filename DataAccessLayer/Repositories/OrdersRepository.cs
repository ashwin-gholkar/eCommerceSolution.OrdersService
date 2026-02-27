using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using MongoDB.Driver;

namespace DataAccessLayer.Repositories
{
    public class OrdersRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly string _collectionName = "Orders";
        public OrdersRepository(IMongoDatabase mongoDatabase)
        {
             _ordersCollection =  mongoDatabase.GetCollection<Order>
                    (_collectionName);
        }

        public async Task<Order?> AddOrder(Order order)
        {
            order.OrderID = Guid.NewGuid();
            order._id = Guid.NewGuid();

            foreach (OrderItem item in order.OrderItems)
            {
                item._id = Guid.NewGuid();
            }

            await _ordersCollection.InsertOneAsync(order); 
            return order;
        }

        public async Task<bool> DeleteOrder(Guid orderID)
        {
            FilterDefinition<Order> filter =
            Builders<Order>.Filter.Eq(o => o.OrderID, orderID);


            Order? existingOrder = (await 
                _ordersCollection.FindAsync(filter))
                    .FirstOrDefault();
            
            if(existingOrder == null)
            {
                return false;
            }

            DeleteResult deleteResult = await _ordersCollection
                .DeleteOneAsync(filter);
            return deleteResult.DeletedCount > 0;
        }

        public async Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter)
        {
            return (await _ordersCollection.FindAsync
                (filter)).FirstOrDefault();
        }

        public Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return (await _ordersCollection.FindAsync
               (Builders<Order>.Filter.Empty))
               .ToList();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCondition(FilterDefinition<Order> filter)
        {
            return (await _ordersCollection.FindAsync
                (filter)).ToList();
        }

        public async Task<Order?> UpdateOrder(Order order)
        {
            FilterDefinition<Order> filter =
             Builders<Order>.Filter.Eq(o => o.OrderID, order.OrderID);


            Order? existingOrder = (await
                _ordersCollection.FindAsync(filter))
                    .FirstOrDefault();

            if (existingOrder == null)
            {
                return null;
            }

            ReplaceOneResult replaceOne =  await
                _ordersCollection.ReplaceOneAsync(filter, order);

            return order;
        }
    }
}
