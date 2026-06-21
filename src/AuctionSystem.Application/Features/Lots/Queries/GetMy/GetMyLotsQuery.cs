using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Lots.Queries.GetMy
{
    public sealed record GetMyLotsQuery() : IRequest<Result<IReadOnlyList<LotDto>>>;
}