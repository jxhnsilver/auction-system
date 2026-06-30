using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Security;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.CreateScheduled
{
    public sealed class CreateScheduledAuctionCommandHandler(
        ICurrentUserProvider currentUserProvider,
        IAuctionRepository auctionRepository,
        ILotRepository lotRepository,
        IAuctionFactory auctionFactory,
        IUnitOfWork uow)
        : IRequestHandler<CreateScheduledAuctionCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateScheduledAuctionCommand request, CancellationToken cancellationToken)
        {
            var sellerId = currentUserProvider.UserId;
            if (sellerId is null)
                return Result<Guid>.Failure(SecurityErrors.Unauthorized());


            if (!LotId.TryParse(request.lotId, out var lotId))
                return Result<Guid>.Failure(LotErrors.InvalidLotId());

            var lot = await lotRepository.GetByIdAsync(lotId, cancellationToken);
            if (lot is null)
                return Result<Guid>.Failure(LotErrors.NotFound(lotId));

            var auctionResult = auctionFactory.CreateScheduled(
                sellerId, 
                lot, 
                request.StartingPrice, 
                request.StartTime, 
                request.EndTime);

            if (auctionResult.IsFailure)
                return Result<Guid>.Failure(auctionResult.Error);

            auctionRepository.Add(auctionResult.Value);
            await uow.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(auctionResult.Value.Id.Value);
        }
    }
}
