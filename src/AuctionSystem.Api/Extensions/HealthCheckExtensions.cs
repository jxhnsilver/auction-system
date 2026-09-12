namespace AuctionSystem.Api.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddHealthChecks()
                .AddNpgSql(
                    config.GetConnectionString("DefaultConnection")!,
                    name: "postgres",
                    timeout: TimeSpan.FromSeconds(3));

            return services;
        }
    }
}
