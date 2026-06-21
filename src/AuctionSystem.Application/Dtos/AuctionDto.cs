namespace AuctionSystem.Application.Dtos
{
    public sealed record AuctionDto(
        Guid Id,
        Guid SellerId,
        Guid LotId,
        decimal StartingPrice,
        decimal CurrentPrice,
        string Status,
        DateTime StartTime,
        DateTime EndTime,
        IReadOnlyList<BidDto> Bids);
}
