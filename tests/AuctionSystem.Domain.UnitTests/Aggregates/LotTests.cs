using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Aggregates.Users;

namespace AuctionSystem.Domain.UnitTests.Aggregates
{
    public class LotTests
    {
        [Fact]
        public void Create_ShouldTrimTitle_AndSetAvailableStatus()
        {
            var lot = Lot.Create(LotId.New(), UserId.New(), "  Vintage Watch  ");

            Assert.Equal("Vintage Watch", lot.Title);
            Assert.Equal(LotStatus.Available, lot.Status);
        }

        [Fact]
        public void MarkAsLocked_ShouldSucceed_WhenLotIsAvailable()
        {
            var lot = Lot.Create(LotId.New(), UserId.New(), "Item");

            var result = lot.MarkAsLocked();

            Assert.True(result.IsSuccess);
            Assert.Equal(LotStatus.OnAuction, lot.Status);
        }

        [Fact]
        public void MarkAsLocked_ShouldFail_WhenLotIsNotAvailable()
        {
            var lot = Lot.Create(LotId.New(), UserId.New(), "Item");
            lot.MarkAsLocked();

            var result = lot.MarkAsLocked();

            Assert.True(result.IsFailure);
            Assert.Equal(LotErrors.InvalidStatus(LotStatus.OnAuction, LotStatus.Available), result.Error);
        }

        [Fact]
        public void MarkAsPendingTransfer_ShouldSucceed_WhenLotIsOnAuction()
        {
            var lot = Lot.Create(LotId.New(), UserId.New(), "Item");
            var lockResult = lot.MarkAsLocked();
            Assert.True(lockResult.IsSuccess);

            var result = lot.MarkAsPendingTransfer();

            Assert.True(result.IsSuccess);
            Assert.Equal(LotStatus.PendingOwnershipTransfer, lot.Status);
        }

        [Fact]
        public void MarkAsAvailable_ShouldSucceed_FromOnAuctionAndPendingTransfer()
        {
            var lot = Lot.Create(LotId.New(), UserId.New(), "Item");
            Assert.True(lot.MarkAsLocked().IsSuccess);

            var result = lot.MarkAsAvailable();

            Assert.True(result.IsSuccess);
            Assert.Equal(LotStatus.Available, lot.Status);
        }

        [Fact]
        public void TransferOwnership_ShouldSucceed_WhenLotIsPendingTransfer()
        {
            var ownerId = UserId.New();
            var newOwnerId = UserId.New();
            var lot = Lot.Create(LotId.New(), ownerId, "Item");

            Assert.True(lot.MarkAsLocked().IsSuccess);
            Assert.True(lot.MarkAsPendingTransfer().IsSuccess);

            var result = lot.TransferOwnership(newOwnerId);

            Assert.True(result.IsSuccess);
            Assert.Equal(newOwnerId, lot.OwnerId);
            Assert.Equal(LotStatus.Available, lot.Status);
        }

        [Fact]
        public void TransferOwnership_ShouldFail_WhenLotIsNotPendingTransfer()
        {
            var lot = Lot.Create(LotId.New(), UserId.New(), "Item");

            var result = lot.TransferOwnership(UserId.New());

            Assert.True(result.IsFailure);
            Assert.Equal(LotErrors.InvalidStatus(LotStatus.Available, LotStatus.PendingOwnershipTransfer), result.Error);
        }
    }
}
