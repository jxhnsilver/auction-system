using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.CreateScheduled
{
    public sealed record CreateScheduledAuctionCommand(
        Guid lotId,
        decimal StartingPrice,
        DateTime StartTime,
        DateTime EndTime) : IRequest<Result<Guid>>;
}
