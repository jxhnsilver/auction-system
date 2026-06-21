namespace AuctionSystem.Application.Dtos
{
    public sealed record AuctionSummaryDto(
        Guid Id,
        Guid SellerId,
        Guid LotId,
        decimal StartingPrice,
        decimal CurrentPrice,
        string Status,
        DateTime StartTime,
        DateTime EndTime);
}