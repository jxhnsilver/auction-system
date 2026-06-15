namespace AuctionSystem.Application.Dtos
{
    public sealed record LotDto(Guid Id, Guid OwnerId, string Title);
}
