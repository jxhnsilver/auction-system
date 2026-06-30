using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Auctions
{
    public static class BidErrors
    {
        public static Error InvalidAmount(decimal amount) => Error.Validation(
            $"Bid amount must be greater than zero. Actual: {amount}");
    }
}