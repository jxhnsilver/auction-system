using AuctionSystem.Domain.Lots;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Users;

namespace AuctionSystem.Domain.Auctions
{
    public sealed class Auction : AggregateRoot<AuctionId>
    {
        // Time buffer for safe scheduling of auctions to avoid scheduling in the past due to clock skew or processing delays
        private static readonly TimeSpan MinimumSchedulingBuffer = TimeSpan.FromSeconds(30);

        private readonly List<Bid> _bids = [];

        public UserId SellerId { get; private set; }
        public LotId LotId { get; private set; }
        public decimal StartingPrice { get; private set; }
        public decimal CurrentPrice { get; private set; }
        public AuctionStatus Status { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public IReadOnlyCollection<Bid> Bids => _bids.AsReadOnly();

        /// <summary>
        /// Parameterless constructor required by EF Core
        /// </summary>
        private Auction() { }

        private Auction(
            AuctionId auctionId,
            UserId sellerId,
            LotId lotId,
            decimal startingPrice,
            DateTime startTime,
            DateTime endTime,
            AuctionStatus status) : base(auctionId)
        {
            SellerId = sellerId;
            LotId = lotId;
            StartingPrice = startingPrice;
            CurrentPrice = startingPrice;
            Status = status;
            StartTime = startTime;
            EndTime = endTime;
        }

        public static Result<Auction> CreateScheduled(
            UserId sellerId,
            LotId lotId,
            decimal startingPrice,
            DateTime startTime,
            DateTime endTime,
            DateTime now)
        {
            TimeSpan timeUntilStart = startTime - now;

            if (timeUntilStart < MinimumSchedulingBuffer)
            {
                var error = timeUntilStart < TimeSpan.Zero
                    ? AuctionErrors.StartTimeCannotBeInPast(startTime)
                    : AuctionErrors.StartTimeTooCloseToPresent(startTime, MinimumSchedulingBuffer);

                return Result<Auction>.Failure(error);
            }

            var validationResult = ValidateCreateInput(sellerId, startingPrice, startTime, endTime);
            if (validationResult.IsFailure)
                return Result<Auction>.Failure(validationResult.Error);

            var auction = new Auction(AuctionId.New(), sellerId, lotId, startingPrice, startTime, endTime, AuctionStatus.Scheduled);

            return Result<Auction>.Success(auction);
        }

        public static Result<Auction> CreateNow(
            UserId sellerId,
            LotId lotId,
            decimal startingPrice,
            DateTime now,
            DateTime endTime)
        {
            var validationResult = ValidateCreateInput(sellerId, startingPrice, now, endTime);
            if (validationResult.IsFailure)
                return Result<Auction>.Failure(validationResult.Error);

            var auction = new Auction(AuctionId.New(), sellerId, lotId, startingPrice, now, endTime, AuctionStatus.Active);

            return Result<Auction>.Success(auction);
        }

        public Result Open(DateTime now)
        {
            if (Status != AuctionStatus.Scheduled)
                return Result.Failure(AuctionErrors.InvalidStatus(Status, AuctionStatus.Scheduled));

            if (now < StartTime)
                return Result.Failure(AuctionErrors.NotStarted(StartTime, now));

            Status = AuctionStatus.Active;

            return Result.Success();
        }

        public Result<Bid> PlaceBid(UserId bidderId, decimal amount, DateTime now)
        {
            if (bidderId is null)
                throw new ArgumentNullException(nameof(bidderId));

            if (Status != AuctionStatus.Active)
                return Result<Bid>.Failure(AuctionErrors.InvalidStatus(Status, AuctionStatus.Active));

            if (now < StartTime || now > EndTime)
                return Result<Bid>.Failure(AuctionErrors.NotInBiddingWindow(now, StartTime, EndTime));

            if (bidderId == SellerId)
                return Result<Bid>.Failure(AuctionErrors.SellerCannotBid());

            if (_bids.Count > 0 && amount <= CurrentPrice)
                return Result<Bid>.Failure(AuctionErrors.MustExceedCurrentPrice(amount, CurrentPrice));

            if (_bids.Count == 0 && amount < StartingPrice)
                return Result<Bid>.Failure(AuctionErrors.MustMeetStartingPrice(amount, StartingPrice));

            var bidResult = Bid.Create(Id, bidderId, amount, now);
            if (bidResult.IsFailure)
                return Result<Bid>.Failure(bidResult.Error);

            var bid = bidResult.Value;
            _bids.Add(bid);
            CurrentPrice = amount;

            return Result<Bid>.Success(bid);
        }

        public Result Close(DateTime now)
        {
            if (Status != AuctionStatus.Active)
                return Result.Failure(AuctionErrors.InvalidStatus(Status, AuctionStatus.Active));

            if (now < EndTime)
                return Result.Failure(AuctionErrors.CannotCloseBeforeEnd(now, EndTime));

            Status = AuctionStatus.Closed;
            return Result.Success();
        }

        private static Result ValidateCreateInput(
            UserId sellerId,
            decimal startingPrice,
            DateTime startTime,
            DateTime endTime)
        {
            if (sellerId is null)
                throw new ArgumentNullException(nameof(sellerId));

            if (startingPrice <= 0)
                return Result.Failure(AuctionErrors.InvalidStartingPrice(startingPrice));

            if (startTime == default)
                throw new ArgumentException("StartTime must be a valid datetime", nameof(startTime));

            if (endTime == default)
                throw new ArgumentException("EndTime must be a valid datetime", nameof(endTime));

            if (endTime <= startTime)
                return Result.Failure(AuctionErrors.InvalidTimeRange(startTime, endTime));

            return Result.Success();
        }
    }
}
