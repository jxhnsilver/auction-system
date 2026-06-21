using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Users;

namespace AuctionSystem.Domain.Auctions
{
    public sealed class Bid : Entity<BidId>
    {
        public AuctionId AuctionId { get; }
        public UserId BidderId { get; }
        public decimal Amount { get; private set; }
        public DateTime PlacedAt { get; }

        /// <summary>
        /// Parameterless constructor required by EF Core
        /// </summary>
        private Bid() { }
        private Bid(BidId id, AuctionId auctionId, UserId bidderId, decimal amount, DateTime placedAt) : base(id)
        {
            AuctionId = auctionId;
            BidderId = bidderId;
            Amount = amount;
            PlacedAt = placedAt;
        }

        public static Result<Bid> Create(AuctionId auctionId, UserId bidderId, decimal amount, DateTime placedAt)
        {
            if (auctionId is null)
                throw new ArgumentNullException(nameof(auctionId));

            if (bidderId is null)
                throw new ArgumentNullException(nameof(bidderId));

            if (placedAt == default)
                throw new ArgumentException("PlacedAt must be a valid datetime", nameof(placedAt));

            if (amount <= 0)
                return Result<Bid>.Failure(BidErrors.InvalidAmount(amount));

            var bid = new Bid(BidId.New(), auctionId, bidderId, amount, placedAt);

            return Result<Bid>.Success(bid);
        }
    }
}
