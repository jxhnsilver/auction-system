using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.Infrastructure.Persistence.Repositories
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        public void Add(User user)
        {
            context.Add(user);  
        }

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
        {
            return await context.Users
                .AsNoTracking()
                .Where(u => u.Email == email)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsync(UserId userId, CancellationToken cancellationToken)
        {
            return await context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync(cancellationToken);
        }
        public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken)
        {
            return await context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }
    }
}
