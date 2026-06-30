using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.Cancel
{
    public sealed record CancelAuctionCommand(Guid AuctionId) : IRequest<Result>;
}
