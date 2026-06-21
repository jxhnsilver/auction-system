using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Lots.Queries.GetAll
{
    public sealed record GetLotsQuery() : IRequest<Result<IReadOnlyList<LotDto>>>;
}