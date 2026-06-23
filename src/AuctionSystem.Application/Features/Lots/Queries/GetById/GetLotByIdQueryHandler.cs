using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Lots;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Lots.Queries.GetById
{
    public class GetLotByIdQueryHandler(ILotRepository lotRepository)
        : IRequestHandler<GetLotByIdQuery, Result<LotDto>>
    {
        public async Task<Result<LotDto>> Handle(GetLotByIdQuery request, CancellationToken cancellationToken)
        {
            if (!LotId.TryParse(request.Id, out var lotId))
                return Result<LotDto>.Failure(Error.Validation("Invalid lot id"));

            var lot = await lotRepository.GetByIdAsync(lotId, cancellationToken);
            if (lot is null)
                return Result<LotDto>.Failure(LotErrors.NotFound(lotId));

            var lotDto = new LotDto(lot.Id.Value, lot.OwnerId.Value, lot.Title, lot.Status.ToString());

            return Result<LotDto>.Success(lotDto);
        }
    }
}