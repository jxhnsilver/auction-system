using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Security;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.Cancel
{
    public sealed class CancelAuctionCommandHandler(
        IAuctionRepository auctionRepository,
        IClock clock,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork uow
        ) : IRequestHandler<CancelAuctionCommand, Result>
    {
        public async Task<Result> Handle(CancelAuctionCommand request, CancellationToken cancellationToken)
        {
            if (!AuctionId.TryParse(request.AuctionId, out var auctionId))
                return Result.Failure(AuctionErrors.InvalidAuctionId());

            var userId = currentUserProvider.UserId;
            if (userId is null)
                return Result.Failure(SecurityErrors.Unauthorized());

            var auction = await auctionRepository.GetByIdAsync(auctionId, cancellationToken);
            if (auction is null)
                return Result.Failure(AuctionErrors.NotFound(auctionId));

            if (userId != auction.SellerId)
                return Result.Failure(SecurityErrors.Forbidden());

            var now = clock.UtcNow;

            var cancelResult = auction.Cancel(now);
            if (cancelResult.IsFailure)
                return cancelResult;

            await uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
