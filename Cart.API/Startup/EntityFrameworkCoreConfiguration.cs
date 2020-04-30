using Cart.Domain;
using Cart.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Cart.API.Startup
{
    public static class EntityFrameworkCoreConfiguration
    {
        public static void AddEfCore(IServiceCollection services)
        {
            services.AddDbContext<ShoppingCartContext>();
            services.AddTransient(typeof(IRepository<,>), typeof(EfRepository<,>));
        }
    }
}