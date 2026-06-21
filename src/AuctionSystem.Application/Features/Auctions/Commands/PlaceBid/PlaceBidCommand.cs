using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.PlaceBid
{
    public sealed record PlaceBidCommand(Guid AuctionId, decimal Amount) : IRequest<Result<Guid>>;
}
