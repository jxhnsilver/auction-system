using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Queries.GetById
{
    public sealed record GetAuctionByIdQuery(Guid Id) : IRequest<Result<AuctionDto>>;
}
