using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Queries.GetAll
{
    public sealed record GetAuctionsQuery() : IRequest<Result<IReadOnlyList<AuctionSummaryDto>>>;
}
