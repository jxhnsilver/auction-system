using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Auctions
{
    public interface IAuctionFactory
    {
        Result<Auction> CreateNow(
            UserId sellerId,
            Lot lot,
            decimal startingPrice,
            DateTime endTime
            );

        Result<Auction> CreateScheduled(
            UserId sellerId,
            Lot lot,
            decimal startingPrice,
            DateTime startTime,
            DateTime endTime
            );
    }
}
