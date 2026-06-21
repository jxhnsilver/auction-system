using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.CreateNow
{
    public sealed record CreateNowAuctionCommand(
        Guid lotId,
        decimal StartingPrice,
        DateTime EndTime) : IRequest<Result<Guid>>;
}
