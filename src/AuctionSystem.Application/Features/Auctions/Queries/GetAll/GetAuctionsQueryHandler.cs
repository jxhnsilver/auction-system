using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Queries.GetAll
{
    public sealed class GetAuctionsQueryHandler(IAuctionRepository auctionRepository)
        : IRequestHandler<GetAuctionsQuery, Result<IReadOnlyList<AuctionSummaryDto>>>
    {
        public async Task<Result<IReadOnlyList<AuctionSummaryDto>>> Handle(GetAuctionsQuery request, CancellationToken cancellationToken)
        {
            var auctions = await auctionRepository.GetAllAsync(cancellationToken);

            var dtos = auctions
                .Select(a => new AuctionSummaryDto(
                    a.Id.Value,
                    a.SellerId.Value,
                    a.LotId.Value,
                    a.StartingPrice,
                    a.CurrentPrice,
                    a.Status.ToString(),
                    a.StartTime,
                    a.EndTime))
                .ToList()
                .AsReadOnly();

            return Result<IReadOnlyList<AuctionSummaryDto>>.Success(dtos);
        }
    }
}
