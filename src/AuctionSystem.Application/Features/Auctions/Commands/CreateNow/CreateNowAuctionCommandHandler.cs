using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Factories;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Security;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.CreateNow
{
    public sealed class CreateNowAuctionCommandHandler(
        ICurrentUserProvider currentUserProvider,
        IAuctionRepository auctionRepository,
        ILotRepository lotRepository,
        IAuctionFactory auctionFactory,
        IUnitOfWork uow)
        : IRequestHandler<CreateNowAuctionCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateNowAuctionCommand request, CancellationToken cancellationToken)
        {
            var sellerId = currentUserProvider.UserId;
            if (sellerId is null)
                return Result<Guid>.Failure(SecurityErrors.Unauthorized());

            if (!LotId.TryParse(request.lotId, out var lotId))
                return Result<Guid>.Failure(LotErrors.InvalidLotId());

            var lot = await lotRepository.GetByIdAsync(lotId, cancellationToken);
            if (lot is null)
                return Result<Guid>.Failure(LotErrors.NotFound(lotId));

            var auctionResult = auctionFactory.CreateNow(
                sellerId,
                lot,
                request.StartingPrice,
                request.EndTime);

            if (auctionResult.IsFailure)
                return Result<Guid>.Failure(auctionResult.Error);

            auctionRepository.Add(auctionResult.Value);
            await uow.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(auctionResult.Value.Id.Value);
        }
    }
}
