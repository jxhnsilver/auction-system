using AuctionSystem.Domain.Lots;
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

        public async Task<Lot?> GetByIdAsync(LotId lotId, CancellationToken cancellationToken)
        {
            return await context.Lots
                .AsNoTracking()
                .SingleOrDefaultAsync(l => l.Id == lotId, cancellationToken);
        }

        public async Task<IReadOnlyList<Lot>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await context.Lots
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}