namespace AuctionSystem.Application.Features.Auctions.Services
{
    public interface IExpiredAuctionCloser
    {
        Task CloseExpiredAsync(CancellationToken cancellationToken);
    }
}
