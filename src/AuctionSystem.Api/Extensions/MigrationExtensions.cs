using AuctionSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.Api.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}
