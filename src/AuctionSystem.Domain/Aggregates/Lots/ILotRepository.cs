using AuctionSystem.Domain.Aggregates.Users;

namespace AuctionSystem.Domain.Aggregates.Lots
{
    public interface ILotRepository
    {
        void Add(Lot lot);
        Task<IReadOnlyList<Lot>> GetAllAsync(CancellationToken cancellationToken);
        Task<Lot?> GetByIdAsync(LotId lotId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Lot>> GetByOwnerAsync(UserId ownerId, CancellationToken cancellationToken);
    }
}