using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Users;

namespace AuctionSystem.Domain.UnitTests.Entities
{
    public class BidTests
    {
        [Fact]
        public void Create_ShouldFail_WhenAmountIsNotPositive()
        {
            var result = Bid.Create(AuctionId.New(), UserId.New(), 0m, DateTime.UtcNow);

            Assert.True(result.IsFailure);
            Assert.Equal(BidErrors.InvalidAmount(0m), result.Error);
        }

        [Fact]
        public void Create_ShouldThrow_WhenAuctionIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Bid.Create(null!, UserId.New(), 100m, DateTime.UtcNow));
        }

        [Fact]
        public void Create_ShouldThrow_WhenBidderIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Bid.Create(AuctionId.New(), null!, 100m, DateTime.UtcNow));
        }

        [Fact]
        public void Create_ShouldThrow_WhenPlacedAtIsDefault()
        {
            Assert.Throws<ArgumentException>(() =>
                Bid.Create(AuctionId.New(), UserId.New(), 100m, default));
        }

        [Fact]
        public void Create_ShouldSucceed_ForValidBid()
        {
            var auctionId = AuctionId.New();
            var bidderId = UserId.New();
            var placedAt = new DateTime(2026, 1, 1, 10, 15, 0, DateTimeKind.Utc);

            var result = Bid.Create(auctionId, bidderId, 150m, placedAt);

            Assert.True(result.IsSuccess);
            Assert.Equal(auctionId, result.Value.AuctionId);
            Assert.Equal(bidderId, result.Value.BidderId);
            Assert.Equal(150m, result.Value.Amount);
            Assert.Equal(placedAt, result.Value.PlacedAt);
        }
    }
}
