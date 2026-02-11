using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {

            string conString = configuration.GetConnectionString("MongoDb")!;
            conString.Replace("$MONGO_HOST",
                Environment.GetEnvironmentVariable("MONGODB_HOST"))
                .Replace("MONGODB_PORT",
                Environment.GetEnvironmentVariable("MONGODB_PORT"));
            // Add other repositories as needed


            services.AddSingleton<IMongoClient>(new MongoClient(conString));

            services.AddScoped<IMongoDatabase>(provider => { });
            return services;
        }
    }
}
