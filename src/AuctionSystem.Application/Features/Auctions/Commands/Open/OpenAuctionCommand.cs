using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.Open
{
    public sealed record OpenAuctionCommand(Guid AuctionId) : IRequest<Result>;
}
