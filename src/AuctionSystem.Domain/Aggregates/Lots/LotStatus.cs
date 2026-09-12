namespace AuctionSystem.Domain.Aggregates.Lots
{
    public enum LotStatus
    {
        Available = 1,
        OnAuction = 2,
        PendingOwnershipTransfer = 3
    }
}
