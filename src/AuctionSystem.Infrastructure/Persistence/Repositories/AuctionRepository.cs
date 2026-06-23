using AuctionSystem.Domain.Auctions;
using AuctionSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.Infrastructure.Persistence.Repositories
{
    public class AuctionRepository(ApplicationDbContext context) : IAuctionRepository
    {
        public void Add(Auction auction)
        {
            context.Add(auction);
        }

        public async Task<Auction?> GetByIdAsync(AuctionId auctionId, CancellationToken cancellationToken)
        {
            return await context.Auctions
                .SingleOrDefaultAsync(a => a.Id == auctionId, cancellationToken);
        }

        public async Task<Auction?> GetByIdWithBidsAsync(AuctionId auctionId, CancellationToken cancellationToken)
        {
            return await context.Auctions
                .Include(a => a.Bids)
                .SingleOrDefaultAsync(a => a.Id == auctionId, cancellationToken);
        }

        public async Task<IReadOnlyList<Auction>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await context.Auctions
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<AuctionId>> GetExpiredIdsAsync(DateTime now, CancellationToken cancellationToken)
        {
            return await context.Auctions
                .Where(a => a.Status == AuctionStatus.Active && a.EndTime <= now)
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CloseBatchAsync(AuctionId[] ids, CancellationToken cancellationToken)
        {
            if (!ids.Any())
                return 0;

            return await context.Auctions
                .Where(a => ids.Contains(a.Id))
                .ExecuteUpdateAsync(
                    s => s.SetProperty(a => a.Status, AuctionStatus.Closed),
                    cancellationToken
                );
        }

        public async Task<int> OpenBatchAsync(AuctionId[] ids, CancellationToken cancellationToken)
        {
            if (!ids.Any())
                return 0;

            return await context.Auctions
                .Where(a => ids.Contains(a.Id))
                .ExecuteUpdateAsync(
                    s => s.SetProperty(a => a.Status, AuctionStatus.Active),
                    cancellationToken
                );
        }

        public async Task<IReadOnlyList<AuctionId>> GetScheduledIdsAsync(DateTime now, CancellationToken cancellationToken)
        {
            return await context.Auctions
                .Where(a => a.Status == AuctionStatus.Scheduled 
                    && a.StartTime <= now
                    && a.EndTime > now)
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
