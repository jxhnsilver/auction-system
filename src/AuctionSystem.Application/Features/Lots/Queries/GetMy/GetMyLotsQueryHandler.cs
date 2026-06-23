using AuctionSystem.Application.Abstractions;
using AuctionSystem.Application.Dtos;
using AuctionSystem.Domain.Lots;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Security;
using MediatR;

namespace AuctionSystem.Application.Features.Lots.Queries.GetMy
{
    public class GetMyLotsQueryHandler(
        ICurrentUserProvider currentUserProvider,
        ILotRepository lotRepository)
        : IRequestHandler<GetMyLotsQuery, Result<IReadOnlyList<LotDto>>>
    {
        public async Task<Result<IReadOnlyList<LotDto>>> Handle(GetMyLotsQuery request, CancellationToken cancellationToken)
        {
            var authenticatedUserId = currentUserProvider.UserId;
            if (authenticatedUserId is null)
                return Result<IReadOnlyList<LotDto>>.Failure(SecurityErrors.Unauthorized());

            var lots = await lotRepository.GetByOwnerAsync(authenticatedUserId, cancellationToken);

            var lotDtos = lots
                .Select(l => new LotDto(l.Id.Value, l.OwnerId.Value, l.Title, l.Status.ToString()))
                .ToList()
                .AsReadOnly();

            return Result<IReadOnlyList<LotDto>>.Success(lotDtos);
        }
    }
}