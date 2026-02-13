using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {

            string conString = configuration.GetConnectionString("MongoDb")!;
            conString.Replace("$MONGO_HOST",
                Environment.GetEnvironmentVariable("MONGODB_HOST"))
                .Replace("MONGO_PORT",
                Environment.GetEnvironmentVariable("MONGODB_PORT"));
            // Add other repositories as needed


            services.AddSingleton<IMongoClient>(new MongoClient(conString));

            services.AddScoped<IMongoDatabase>(provider => {

                IMongoClient client =
                provider.GetRequiredService<IMongoClient>();
                return client.GetDatabase("OrdersDatabase"); 
            });

            services.AddScoped<IOrderRepository, 
                OrdersRepository>();

            return services;
        }
    }
}
