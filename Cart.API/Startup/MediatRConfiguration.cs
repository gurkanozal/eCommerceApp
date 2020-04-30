using Cart.Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Cart.API.Startup
{
    public class MediatRConfiguration
    {
        public static void AddMediatR(IServiceCollection services)
        {
            services.AddMediatR(typeof(RequestValidationException).Assembly)
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
        }
    }
}