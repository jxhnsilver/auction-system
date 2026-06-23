using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Lots;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Lots.Queries.GetAll
{
    public class GetLotsQueryHandler(ILotRepository lotRepository)
        : IRequestHandler<GetLotsQuery, Result<IReadOnlyList<LotDto>>>
    {
        public async Task<Result<IReadOnlyList<LotDto>>> Handle(GetLotsQuery request, CancellationToken cancellationToken)
        {
            var lots = await lotRepository.GetAllAsync(cancellationToken);

            var lotDtos = lots
                .Select(l => new LotDto(l.Id.Value, l.OwnerId.Value, l.Title, l.Status.ToString()))
                .ToList()
                .AsReadOnly();

            return Result<IReadOnlyList<LotDto>>.Success(lotDtos);
        }
    }
}