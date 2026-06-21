using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.Close
{
    public sealed record CloseAuctionCommand(Guid AuctionId) : IRequest<Result>;
}
