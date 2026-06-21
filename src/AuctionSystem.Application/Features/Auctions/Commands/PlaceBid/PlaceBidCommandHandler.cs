using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Auctions;
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
        : IRequestHandler<PlaceBidCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
        {
            if (!AuctionId.TryParse(request.AuctionId, out var auctionId))
                return Result<Guid>.Failure(AuctionErrors.InvalidAuctionId());

            var bidderId = currentUserProvider.UserId;
            if (bidderId is null)
                return Result<Guid>.Failure(SecurityErrors.Unauthorized());

            var auction = await auctionRepository.GetByIdWithBidsAsync(auctionId, cancellationToken);
            if (auction is null)
                return Result<Guid>.Failure(AuctionErrors.NotFound(auctionId));

            if (auction.SellerId == bidderId)
                return Result<Guid>.Failure(AuctionErrors.SellerCannotBid());

            var bidResult = auction.PlaceBid(bidderId, request.Amount, clock.UtcNow);
            if (bidResult.IsFailure)
                return Result<Guid>.Failure(bidResult.Error);

            await uow.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(bidResult.Value.Id.Value);
        }
    }
}
