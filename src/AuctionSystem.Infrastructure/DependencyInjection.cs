using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Aggregates.Wallets;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Infrastructure.Authentication;
using AuctionSystem.Infrastructure.Authentication.Jwt;
using AuctionSystem.Infrastructure.BackgroundServices;
using AuctionSystem.Infrastructure.Extensions;
using AuctionSystem.Infrastructure.Persistence.Context;
using AuctionSystem.Infrastructure.Persistence.Repositories;
using AuctionSystem.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string is not configured");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)).UseSnakeCaseNamingConvention());

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<ILotRepository, LotRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();

            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddJwtAuthentication(configuration);
            services.AddSingleton<ITokenProvider, JwtTokenProvider>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

            services.AddSingleton<IClock, SystemClock>();

            services.AddHostedService<ExpiredAuctionCloserWorker>();
            services.AddHostedService<ScheduledAuctionOpenerWorker>();

            return services;
        }
    }
}
