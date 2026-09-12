using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Factories;
using AuctionSystem.Domain.UnitTests.Fakes;

namespace AuctionSystem.Domain.UnitTests.Factories
{
    public class AuctionFactoryTests
    {
        [Fact]
        public void CreateNow_ShouldCreateActiveAuction_AndLockLot()
        {
            var now = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var clock = new FakeClock(now);
            var factory = new AuctionFactory(clock);

            var sellerId = UserId.New();
            var lot = Lot.Create(LotId.New(), sellerId, "Item");
            var endTime = now.AddHours(2);

            var result = factory.CreateNow(sellerId, lot, 100m, endTime);

            Assert.True(result.IsSuccess);
            Assert.Equal(AuctionStatus.Active, result.Value.Status);
            Assert.Equal(now, result.Value.StartTime);
            Assert.Equal(endTime, result.Value.EndTime);
            Assert.Equal(LotStatus.OnAuction, lot.Status);
        }

        [Fact]
        public void CreateScheduled_ShouldCreateScheduledAuction_AndLockLot()
        {
            var now = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var clock = new FakeClock(now);
            var factory = new AuctionFactory(clock);

            var sellerId = UserId.New();
            var lot = Lot.Create(LotId.New(), sellerId, "Item");
            var startTime = now.AddMinutes(40);
            var endTime = now.AddHours(2);

            var result = factory.CreateScheduled(sellerId, lot, 100m, startTime, endTime);

            Assert.True(result.IsSuccess);
            Assert.Equal(AuctionStatus.Scheduled, result.Value.Status);
            Assert.Equal(startTime, result.Value.StartTime);
            Assert.Equal(endTime, result.Value.EndTime);
            Assert.Equal(LotStatus.OnAuction, lot.Status);
        }
    }
}
