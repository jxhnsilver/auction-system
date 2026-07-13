using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Users;

namespace AuctionSystem.Domain.Aggregates.Wallets
{
    public interface IWalletRepository
    {
        Task<Wallet> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken);
        Task<Wallet> GetByOwnerIdWithActiveHoldAsync(UserId userId, AuctionId auctionId, CancellationToken cancellationToken);
        void Add(Wallet wallet);
    }
}
