namespace AuctionSystem.Domain.Auctions
{
    public interface IAuctionRepository
    {
        void Add(Auction auction);
        Task<Auction?> GetByIdAsync(AuctionId auctionId, CancellationToken cancellationToken);
        Task<Auction?> GetByIdWithBidsAsync(AuctionId auctionId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Auction>> GetAllAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<AuctionId>> GetExpiredIdsAsync(DateTime now, CancellationToken cancellationToken);
        Task<IReadOnlyList<AuctionId>> GetScheduledIdsAsync(DateTime now, CancellationToken cancellationToken);
        Task<int> CloseBatchAsync(AuctionId[] ids, CancellationToken cancellationToken);
        Task<int> OpenBatchAsync(AuctionId[] ids, CancellationToken cancellationToken);
    }
}
