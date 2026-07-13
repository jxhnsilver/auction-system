using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.Infrastructure.Persistence.Repositories
{
    public class LotRepository(ApplicationDbContext context) : ILotRepository
    {
        public void Add(Lot lot)
        {
            context.Add(lot);
        }

        public async Task<IReadOnlyList<Lot>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await context.Lots
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Lot?> GetByIdAsync(LotId lotId, CancellationToken cancellationToken)
        {
            return await context.Lots
                .SingleOrDefaultAsync(l => l.Id == lotId, cancellationToken);
        }

        public async Task<IReadOnlyList<Lot>> GetByOwnerAsync(UserId ownerId, CancellationToken cancellationToken)
        {
            return await context.Lots
                .AsNoTracking()
                .Where(l => l.OwnerId == ownerId)
                .ToListAsync(cancellationToken);
        }
    }
}