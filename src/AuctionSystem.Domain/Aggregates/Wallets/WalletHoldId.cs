using System.Diagnostics.CodeAnalysis;

namespace AuctionSystem.Domain.Aggregates.Wallets
{
    public sealed record WalletHoldId
    {
        public Guid Value { get; }

        private WalletHoldId(Guid value)
        {
            Value = value;
        }

        public static WalletHoldId New() => new WalletHoldId(Guid.NewGuid());
        public static WalletHoldId From(Guid walletHoldId)
        {
            if (walletHoldId == Guid.Empty)
                throw new ArgumentException("Wallet ID cannot be empty", nameof(walletHoldId));

            return new WalletHoldId(walletHoldId);
        }
        public static bool TryParse(Guid guid, [NotNullWhen(true)] out WalletHoldId? walletHoldId)
        {
            if (guid == Guid.Empty)
            {
                walletHoldId = null;
                return false;
            }

            walletHoldId = new WalletHoldId(guid);

            return true;
        }
    }
}
