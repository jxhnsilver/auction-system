using System.Diagnostics.CodeAnalysis;

namespace AuctionSystem.Domain.Auctions
{
    public sealed record AuctionId
    {
        public Guid Value { get; }

        private AuctionId(Guid value)
        {
            Value = value;
        }

        public static AuctionId New() => new AuctionId(Guid.NewGuid());
        public static AuctionId From(Guid auctionId)
        {
            if (auctionId == Guid.Empty)
                throw new ArgumentException("Auction ID cannot be empty", nameof(auctionId));

            return new AuctionId(auctionId);
        }
        public static bool TryParse(Guid guid, [NotNullWhen(true)] out AuctionId? auctionId)
        {
            if (guid == Guid.Empty)
            {
                auctionId = null;
                return false;
            }

            auctionId = new AuctionId(guid);
            return true;
        }
    }
}
