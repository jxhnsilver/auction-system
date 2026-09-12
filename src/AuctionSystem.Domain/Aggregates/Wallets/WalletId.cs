using System.Diagnostics.CodeAnalysis;

namespace AuctionSystem.Domain.Aggregates.Wallets
{
    public sealed record WalletId
    {
        public Guid Value { get; }

        private WalletId(Guid value)
        {
            Value = value;
        }

        public static WalletId New() => new WalletId(Guid.NewGuid());
        public static WalletId From(Guid walletId)
        {
            if (walletId == Guid.Empty)
                throw new ArgumentException("Wallet ID cannot be empty", nameof(walletId));

            return new WalletId(walletId);
        }
        public static bool TryParse(Guid guid, [NotNullWhen(true)] out WalletId? walletId)
        {
            if (guid == Guid.Empty)
            {
                walletId = null;
                return false;
            }

            walletId = new WalletId(guid);
            return true;
        }
    }
}
