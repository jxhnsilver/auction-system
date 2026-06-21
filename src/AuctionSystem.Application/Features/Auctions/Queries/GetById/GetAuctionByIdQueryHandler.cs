using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Auctions;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Queries.GetById
{
    public sealed class GetAuctionByIdQueryHandler(IAuctionRepository auctionRepository)
        : IRequestHandler<GetAuctionByIdQuery, Result<AuctionDto>>
    {
        public async Task<Result<AuctionDto>> Handle(GetAuctionByIdQuery request, CancellationToken cancellationToken)
        {
            if (!AuctionId.TryParse(request.Id, out var auctionId))
                return Result<AuctionDto>.Failure(AuctionErrors.InvalidAuctionId());

            var auction = await auctionRepository.GetByIdWithBidsAsync(auctionId, cancellationToken);
            if (auction is null)
                return Result<AuctionDto>.Failure(AuctionErrors.NotFound(auctionId));

            var dto = new AuctionDto(
                auction.Id.Value,
                auction.SellerId.Value,
                auction.LotId.Value,
                auction.StartingPrice,
                auction.CurrentPrice,
                auction.Status.ToString(),
                auction.StartTime,
                auction.EndTime,
                auction.Bids.Select(b => new BidDto(
                    b.Id.Value,
                    b.AuctionId.Value,
                    b.BidderId.Value,
                    b.Amount,
                    b.PlacedAt)).ToList().AsReadOnly());

            return Result<AuctionDto>.Success(dto);
        }
    }
}
