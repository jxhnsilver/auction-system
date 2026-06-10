namespace AuctionSystem.Domain.Lots
{
    public interface ILotRepository
    {
        void Add(Lot lot);
        Task<Lot?> GetByIdAsync(LotId lotId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Lot>> GetAllAsync(CancellationToken cancellationToken);
    }
}