using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Lots
{
    public static class LotErrors
    {
        public static Error InvalidLotId() => Error.Validation("Invalid lot id");
        public static Error InvalidStatus(LotStatus current, params LotStatus[] expected) => Error.Conflict(
            $"Invalid lot status. Current: {current}. Expected: {string.Join(", ", expected)}");
        public static Error NotFound(LotId lotId) => Error.NotFound(
        $"The Lot with the Id = '{lotId.Value}' was not found");
    }
}
