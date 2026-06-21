using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Lots.Queries.GetById
{
    public sealed record GetLotByIdQuery(Guid Id) : IRequest<Result<LotDto>>;
}