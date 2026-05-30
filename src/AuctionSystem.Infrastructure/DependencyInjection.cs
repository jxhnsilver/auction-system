using AuctionSystem.Application.Abstractions;
using AuctionSystem.Infrastructure.Persistence.Context;
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
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            JwtSettingsValidator.Validate(jwtSettings!);

            services.AddSingleton<ITokenProvider, JwtTokenProvider>();
            services.AddJwtAuthentication(configuration);
            return services;
        }
    }
}
