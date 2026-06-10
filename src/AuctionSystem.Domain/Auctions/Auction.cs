using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Users;

namespace AuctionSystem.Domain.Auctions
{
    public sealed class Auction : AggregateRoot<AuctionId>
    {
        private readonly List<Bid> _bids = [];

        public UserId SellerId { get; private set; }
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
            decimal startingPrice,
            DateTime startTime,
            DateTime endTime) : base(auctionId)
        {
            SellerId = sellerId;
            StartingPrice = startingPrice;
            CurrentPrice = startingPrice;
            Status = AuctionStatus.Draft;
            StartTime = startTime;
            EndTime = endTime;
        }

        public static Result<Auction> Create(
            UserId sellerId,
            decimal startingPrice,
            DateTime startTime,
            DateTime endTime)
        {
            if (sellerId is null)
                throw new ArgumentNullException(nameof(sellerId));

            if (startingPrice <= 0)
                return Result<Auction>.Failure(AuctionErrors.InvalidStartingPrice(startingPrice));

            if (startTime == default)
                throw new ArgumentException("StartTime must be a valid datetime", nameof(startTime));

            if (endTime == default)
                throw new ArgumentException("EndTime must be a valid datetime", nameof(endTime));

            if (endTime <= startTime)
                return Result<Auction>.Failure(AuctionErrors.InvalidTimeRange(startTime, endTime));

            var auction = new Auction(AuctionId.New(), sellerId, startingPrice, startTime, endTime);

            return Result<Auction>.Success(auction);
        }

        public Result Open(DateTime now)
        {
            if (now == default)
                throw new ArgumentException("Now must be a valid datetime", nameof(now));

            if (Status != AuctionStatus.Draft)
                return Result.Failure(AuctionErrors.InvalidStatus(Status, AuctionStatus.Draft));

            if (now < StartTime)
                return Result.Failure(AuctionErrors.NotStarted(StartTime, now));

            Status = AuctionStatus.Active;

            return Result.Success();
        }

        public Result<Bid> PlaceBid(UserId bidderId, decimal amount, DateTime now)
        {
            if (bidderId is null)
                throw new ArgumentNullException(nameof(bidderId));

            if (now == default)
                throw new ArgumentException("Now must be a valid datetime", nameof(now));

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
            if (now == default)
                throw new ArgumentException("Now must be a valid datetime", nameof(now));

            if (Status != AuctionStatus.Active)
                return Result.Failure(AuctionErrors.InvalidStatus(Status, AuctionStatus.Active));

            if (now < EndTime)
                return Result.Failure(AuctionErrors.CannotCloseBeforeEnd(now, EndTime));

            Status = AuctionStatus.Closed;
            return Result.Success();
        }
    }
}
