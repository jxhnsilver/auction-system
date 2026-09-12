using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Lots
{
    public sealed class Lot : AggregateRoot<LotId>
    {
        public UserId OwnerId { get; private set; }
        public string Title { get; private set; }
        public LotStatus Status { get; private set; }

        private Lot() { }
        private Lot(LotId lotId, UserId ownerId, string title) : base(lotId)
        {
            OwnerId = ownerId;
            Title = title.Trim();
            Status = LotStatus.Available;
        }

        public static Lot Create(LotId lotId, UserId ownerId, string title)
        {
            if (lotId is null)
                throw new ArgumentNullException(nameof(lotId));
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            return new Lot(lotId, ownerId, title);
        }
        public Result MarkAsLocked()
        {
            if (Status != LotStatus.Available)
                return Result.Failure(LotErrors.InvalidStatus(Status, LotStatus.Available));

            Status = LotStatus.OnAuction;

            return Result.Success();
        }
        public Result MarkAsAvailable()
        {
            if (Status != LotStatus.OnAuction && Status != LotStatus.PendingOwnershipTransfer)
                return Result.Failure(LotErrors.InvalidStatus(Status, LotStatus.OnAuction, LotStatus.PendingOwnershipTransfer));

            Status = LotStatus.Available;

            return Result.Success();
        }
        public Result MarkAsPendingTransfer()
        {
            if (Status != LotStatus.OnAuction)
                return Result.Failure(LotErrors.InvalidStatus(Status, LotStatus.OnAuction));

            Status = LotStatus.PendingOwnershipTransfer;
            return Result.Success();
        }
        public Result TransferOwnership(UserId newOwnerId)
        {
            if (newOwnerId is null)
                throw new ArgumentNullException(nameof(newOwnerId));

            if (Status != LotStatus.PendingOwnershipTransfer)
                return Result.Failure(LotErrors.InvalidStatus(Status, LotStatus.PendingOwnershipTransfer));

            OwnerId = newOwnerId;
            Status = LotStatus.Available;

            return Result.Success();
        }
    }
}
