using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Auctions
{
    public sealed class Auction : AggregateRoot<AuctionId>
    {
        // Time buffer for safe scheduling of auctions to avoid scheduling in the past due to clock skew or processing delays
        internal static readonly TimeSpan MinimumSchedulingBuffer = TimeSpan.FromSeconds(30);

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

        internal Auction(
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

        public Result PlaceBid(UserId bidderId, decimal amount, DateTime now)
        {
            if (bidderId is null)
                throw new ArgumentNullException(nameof(bidderId));

            if (Status != AuctionStatus.Active)
                return Result.Failure(AuctionErrors.InvalidStatus(Status, AuctionStatus.Active));

            if (now < StartTime || now > EndTime)
                return Result.Failure(AuctionErrors.NotInBiddingWindow(now, StartTime, EndTime));

            if (bidderId == SellerId)
                return Result.Failure(AuctionErrors.SellerCannotBid());

            if (_bids.Count > 0 && amount <= CurrentPrice)
                return Result.Failure(AuctionErrors.MustExceedCurrentPrice(amount, CurrentPrice));

            if (_bids.Count == 0 && amount < StartingPrice)
                return Result.Failure(AuctionErrors.MustMeetStartingPrice(amount, StartingPrice));

            var bidResult = Bid.Create(Id, bidderId, amount, now);
            if (bidResult.IsFailure)
                return Result.Failure(bidResult.Error);

            var bid = bidResult.Value;
            _bids.Add(bid);
            CurrentPrice = amount;

            return Result.Success();
        }

        public Result Cancel(DateTime now)
        {
            if (now > EndTime || Status == AuctionStatus.Closed)
                return Result.Failure(AuctionErrors.AlreadyClosed());

            if (Status == AuctionStatus.Cancelled)
                return Result.Failure(AuctionErrors.AlreadyCancelled());

            if (_bids.Any())
                return Result.Failure(AuctionErrors.CannotCancelWithBids());

            Status = AuctionStatus.Cancelled;

            return Result.Success();
        }
    }
}
