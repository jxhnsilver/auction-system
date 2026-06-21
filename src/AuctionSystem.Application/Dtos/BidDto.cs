namespace AuctionSystem.Application.Dtos
{
    public sealed record BidDto(
        Guid Id,
        Guid AuctionId,
        Guid BidderId,
        decimal Amount,
        DateTime PlacedAt);
}
