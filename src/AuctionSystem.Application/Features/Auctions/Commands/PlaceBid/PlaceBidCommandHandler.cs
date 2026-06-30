using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Security;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.PlaceBid
{
    public sealed class PlaceBidCommandHandler(
        ICurrentUserProvider currentUserProvider,
        IAuctionRepository auctionRepository,
        IClock clock,
        IUnitOfWork uow)
        : IRequestHandler<PlaceBidCommand, Result>
    {
        public async Task<Result> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
        {
            if (!AuctionId.TryParse(request.AuctionId, out var auctionId))
                return Result.Failure(AuctionErrors.InvalidAuctionId());

            var bidderId = currentUserProvider.UserId;
            if (bidderId is null)
                return Result.Failure(SecurityErrors.Unauthorized());

            var auction = await auctionRepository.GetByIdWithBidsAsync(auctionId, cancellationToken);
            if (auction is null)
                return Result.Failure(AuctionErrors.NotFound(auctionId));

            if (auction.SellerId == bidderId)
                return Result.Failure(AuctionErrors.SellerCannotBid());

            var now = clock.UtcNow;

            var bidResult = auction.PlaceBid(bidderId, request.Amount, now);
            if (bidResult.IsFailure)
                return bidResult;

            await uow.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
