using AuctionSystem.Presentation.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionSystem.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddEndpoints();

            return services;
        }
    }
}
