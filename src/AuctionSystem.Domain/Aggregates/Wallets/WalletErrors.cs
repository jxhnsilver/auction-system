using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Wallets
{
    public static class WalletErrors
    {
        public static Error InvalidWalletId() => Error.Validation("Invalid wallet id");
        public static Error InvalidOwnerId() => Error.Validation("Invalid wallet owner id");
        public static Error InvalidAmount(decimal amount) => Error.Validation(
            $"Amount must be greater than zero. Actual: {amount}");
        public static Error InsufficientFunds(decimal requested, decimal available) => Error.Conflict(
            $"Insufficient funds. Requested: {requested}, Available: {available}");
        public static Error ActiveHoldAlreadyExists(AuctionId auctionId) => Error.Conflict(
            $"An active hold for auction '{auctionId.Value}' already exists");
        public static Error NoActiveHoldForAuction(AuctionId auctionId) => Error.NotFound(
            $"No active hold found for auction '{auctionId.Value}'");
        public static Error InvalidHoldStatus(string current, string expected) => Error.Conflict(
            $"Invalid hold status. Current: {current}. Expected: {expected}");
    }
}
