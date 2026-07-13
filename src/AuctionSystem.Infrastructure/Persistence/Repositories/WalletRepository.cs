using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Aggregates.Wallets;
using AuctionSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.Infrastructure.Persistence.Repositories
{
    public class WalletRepository(ApplicationDbContext context) : IWalletRepository
    {
        public async Task<Wallet> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken)
        {
            return await context.Wallets
                .SingleOrDefaultAsync(w => w.OwnerId == ownerId, cancellationToken)
                ?? throw new InvalidOperationException($"Wallet for user {ownerId} not found"); ;
        }

        public async Task<Wallet> GetByOwnerIdWithActiveHoldAsync(UserId userId, AuctionId auctionId, CancellationToken cancellationToken)
        {
            return await context.Wallets
                .Include(w => w.Holds.Where(h => h.AuctionId == auctionId && h.Status == WalletHoldStatus.Active))
                .SingleOrDefaultAsync(w => w.OwnerId == userId, cancellationToken)
                ?? throw new InvalidOperationException($"Wallet for user {userId} not found"); ;
        }

        public void Add(Wallet wallet)
        {
            context.Add(wallet);
        }
    }
}
