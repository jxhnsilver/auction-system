using AuctionSystem.Application.Features.Auctions.Services;
using AuctionSystem.Domain.Auctions;
using AuctionSystem.Domain.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            services.AddScoped<IExpiredAuctionCloser, ExpiredAuctionCloser>();
            services.AddScoped<IScheduledAuctionOpener, ScheduledAuctionOpener>();

            services.AddScoped<IAuctionFactory, AuctionFactory>();

            return services;
        }
    }
}
