using AuctionSystem.Domain.Lots;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Users;

namespace AuctionSystem.Domain.Auctions
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
