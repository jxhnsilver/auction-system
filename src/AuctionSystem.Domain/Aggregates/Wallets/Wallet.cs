using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Wallets
{
    public sealed class Wallet : AggregateRoot<WalletId>
    {
        private readonly List<WalletHold> _holds = [];

        public UserId OwnerId { get; private set; }
        public decimal Balance { get; private set; }
        public decimal LockedBalance { get; private set; }
        public decimal AvailableBalance => Balance - LockedBalance;
        public IReadOnlyCollection<WalletHold> Holds => _holds.AsReadOnly();

        private Wallet() { }

        private Wallet(WalletId walletId, UserId ownerId) : base(walletId)
        {
            OwnerId = ownerId;
            Balance = 0m;
            LockedBalance = 0m;
        }

        public static Result<Wallet> Create(UserId ownerId)
        {
            if (ownerId is null)
                return Result<Wallet>.Failure(WalletErrors.InvalidOwnerId());

            var wallet = new Wallet(WalletId.New(), ownerId);

            return Result<Wallet>.Success(wallet);
        }

        public Result FreezeFunds(AuctionId auctionId, decimal amount, DateTime now)
        {
            if (auctionId is null) 
                throw new ArgumentNullException(nameof(auctionId));

            if (now == default) 
                throw new ArgumentException("Freeze time must be valid", nameof(now));

            if (amount <= 0)
                return Result.Failure(WalletErrors.InvalidAmount(amount));

            if (GetActiveHold(auctionId) is not null)
                return Result.Failure(WalletErrors.ActiveHoldAlreadyExists(auctionId));

            if (amount > AvailableBalance)
                return Result.Failure(WalletErrors.InsufficientFunds(amount, AvailableBalance));

            var holdResult = WalletHold.Create(Id, auctionId, amount, now);
            if (holdResult.IsFailure)
                return Result.Failure(holdResult.Error);

            var hold = holdResult.Value;
            _holds.Add(hold);

            LockedBalance += amount;

            return Result.Success();
        }

        public Result UnfreezeFunds(AuctionId auctionId, DateTime now)
        {
            if (auctionId is null)
                throw new ArgumentNullException(nameof(auctionId));

            if (now == default)
                throw new ArgumentException("Unfreeze time must be valid", nameof(now));

            var hold = GetActiveHold(auctionId);
            if (hold is null)
                return Result.Failure(WalletErrors.NoActiveHoldForAuction(auctionId));

            var releaseResult = hold.Release(now);
            if (releaseResult.IsFailure)
                return Result.Failure(releaseResult.Error);

            LockedBalance -= hold.Amount;

            return Result.Success();
        }

        public Result CaptureFunds(AuctionId auctionId, DateTime now)
        {
            if (auctionId is null)
                throw new ArgumentNullException(nameof(auctionId));

            if (now == default)
                throw new ArgumentException("Capture time must be valid", nameof(now));

            var hold = GetActiveHold(auctionId);
            if (hold is null)
                return Result.Failure(WalletErrors.NoActiveHoldForAuction(auctionId));

            if (hold.Amount > Balance)
                return Result.Failure(WalletErrors.InsufficientFunds(hold.Amount, Balance));

            var captureResult = hold.Capture(now);
            if (captureResult.IsFailure)
                return Result.Failure(captureResult.Error);

            LockedBalance -= hold.Amount;
            Balance -= hold.Amount;

            return Result.Success();
        }

        private WalletHold? GetActiveHold(AuctionId auctionId)
            => _holds.FirstOrDefault(hold => hold.AuctionId == auctionId && hold.Status == WalletHoldStatus.Active);
    }
}
