namespace AuctionSystem.Domain.Auctions
{
    public sealed record BidId
    {
        public Guid Value { get; }

        private BidId(Guid value)
        {
            Value = value;
        }

        public static BidId New() => new BidId(Guid.NewGuid());
        public static BidId From(Guid bidId)
        {
            if (bidId == Guid.Empty)
                throw new ArgumentException("Bid ID cannot be empty", nameof(bidId));

            return new BidId(bidId);
        }
    }
}
