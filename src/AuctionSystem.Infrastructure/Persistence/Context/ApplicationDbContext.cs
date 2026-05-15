using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
