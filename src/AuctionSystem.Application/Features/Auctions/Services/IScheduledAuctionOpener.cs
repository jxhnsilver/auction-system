namespace AuctionSystem.Application.Features.Auctions.Services
{
    public interface IScheduledAuctionOpener
    {
        Task OpenScheduledAsync(CancellationToken cancellationToken);
    }
}
