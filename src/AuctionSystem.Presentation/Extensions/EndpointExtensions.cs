using AuctionSystem.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AuctionSystem.Presentation.Extensions
{
    public static class EndpointExtensions
    {
        public static IServiceCollection AddEndpoints(this IServiceCollection services)
        {
            // Find all non-abstract classes implementing IEndpoint in the current assembly
            var endpointTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass &&        // Must be a class
                           !t.IsAbstract &&     // // Must be not abstract
                           typeof(IEndpoint).IsAssignableFrom(t)); // Must implement IEndpoint

            foreach (var type in endpointTypes)
            {
                services.AddSingleton(typeof(IEndpoint), type);
            }

            return services;
        }
        public static void MapEndpoints(this WebApplication app)
        {
            var endpoints = app.Services.GetServices<IEndpoint>();

            foreach (var endpoint in endpoints)
            {
                endpoint.MapEndpoint(app);
            }
        }
    }
}
