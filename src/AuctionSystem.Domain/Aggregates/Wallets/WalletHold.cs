using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Wallets
{
    public sealed class WalletHold : Entity<WalletHoldId>
    {
        public WalletId WalletId { get; private set; }
        public AuctionId AuctionId { get; private set; }
        public decimal Amount { get; private set; }
        public WalletHoldStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        private WalletHold() { }

        private WalletHold(WalletHoldId walletHoldId, WalletId walletId, AuctionId auctionId, decimal amount, DateTime createdAt) 
            : base(walletHoldId)
        {
            WalletId = walletId;
            AuctionId = auctionId;
            Amount = amount;
            CreatedAt = createdAt;
            Status = WalletHoldStatus.Active;
        }

        public static Result<WalletHold> Create(WalletId walletId, AuctionId auctionId, decimal amount, DateTime createdAt)
        {
            if (walletId is null)
                throw new ArgumentNullException(nameof(walletId));

            if (auctionId is null)
                throw new ArgumentNullException(nameof(auctionId));

            if (amount <= 0)
                return Result<WalletHold>.Failure(WalletHoldErrors.InvalidAmount(amount));

            if (createdAt == default)
                throw new ArgumentException("CreatedAt must be valid", nameof(createdAt));

            var hold = new WalletHold(WalletHoldId.New(), walletId, auctionId, amount, createdAt);

            return Result<WalletHold>.Success(hold);
        }

        /// <summary>
        /// Transitions the hold status to 'Released', indicating the funds are no longer required for this bid.
        /// This method only updates the internal state of the hold. The Wallet aggregate uses this status change 
        /// to decrease its LockedBalance, which automatically increases the calculated AvailableBalance.
        /// Typically called when a bid is outbid.
        /// </summary>
        /// <param name="now">The current UTC time when the release occurs.</param>
        /// <returns>A success result if the hold was active and released; otherwise, a failure result.</returns>
        public Result Release(DateTime now)
        {
            if (now == default)
                throw new ArgumentException("Release time must be valid", nameof(now));

            if (Status != WalletHoldStatus.Active)
                return Result.Failure(WalletErrors.InvalidHoldStatus(Status.ToString(), WalletHoldStatus.Active.ToString()));

            Status = WalletHoldStatus.Released;
            CompletedAt = now;

            return Result.Success();
        }

        /// <summary>
        /// Transitions the hold status to 'Captured', indicating the funds have been successfully claimed.
        /// This method only updates the internal state of the hold. The Wallet aggregate uses this status change 
        /// to permanently decrease its LockedBalance and total Balance.
        /// Typically called when the user wins the auction.
        /// </summary>
        /// <param name="now">The current UTC time when the capture occurs.</param>
        /// <returns>A success result if the hold was active and captured; otherwise, a failure result.</returns>
        public Result Capture(DateTime now)
        {
            if (now == default)
                throw new ArgumentException("Capture time must be valid", nameof(now));

            if (Status != WalletHoldStatus.Active)
                return Result.Failure(WalletErrors.InvalidHoldStatus(Status.ToString(), WalletHoldStatus.Active.ToString()));

            Status = WalletHoldStatus.Captured;
            CompletedAt = now;

            return Result.Success();
        }
    }

}
