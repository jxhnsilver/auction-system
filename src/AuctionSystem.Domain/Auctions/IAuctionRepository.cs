using AuctionSystem.Domain.Users;

namespace AuctionSystem.Domain.Auctions
{
    public interface IAuctionRepository
    {
        void Add(Auction auction);
        Task<Auction?> GetByIdAsync(AuctionId auctionId, CancellationToken cancellationToken);
        Task<Auction?> GetByIdWithBidsAsync(AuctionId auctionId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Auction>> GetAllAsync(CancellationToken cancellationToken);
    }
}
