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
                .Include(a => a.Bids)
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.Id == auctionId, cancellationToken);
        }

        public async Task<IReadOnlyList<Auction>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await context.Auctions
                .Include(a => a.Bids)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}