using System.Diagnostics.CodeAnalysis;

namespace AuctionSystem.Domain.Lots
{
    public sealed record LotId
    {
        public Guid Value { get; }

        private LotId(Guid value)
        {
            Value = value;
        }

        public static LotId New() => new LotId(Guid.NewGuid());
        public static LotId From(Guid lotId)
        {
            if (lotId == Guid.Empty)
                throw new ArgumentException("Lot ID cannot be empty", nameof(lotId));

            return new LotId(lotId);
        }
        public static bool TryParse(Guid guid, [NotNullWhen(true)] out LotId? lotId)
        {
            if (guid == Guid.Empty)
            {
                lotId = null;
                return false;
            }

            lotId = new LotId(guid);
            return true;
        }
    }
}
