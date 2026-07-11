using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Wallets
{
    public static class WalletHoldErrors
    {
        public static Error InvalidAmount(decimal amount) => Error.Validation(
            $"Wallet hold amount must be greater than zero. Actual: {amount}");
    }
}
