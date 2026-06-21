using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Auctions;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.Close
{
    public sealed class CloseAuctionCommandHandler(
        IAuctionRepository auctionRepository,
        IClock clock,
        IUnitOfWork uow)
        : IRequestHandler<CloseAuctionCommand, Result>
    {
        public async Task<Result> Handle(CloseAuctionCommand request, CancellationToken cancellationToken)
        {
            if (!AuctionId.TryParse(request.AuctionId, out var auctionId))
                return Result.Failure(AuctionErrors.InvalidAuctionId());

            var auction = await auctionRepository.GetByIdAsync(auctionId, cancellationToken);
            if (auction is null)
                return Result.Failure(AuctionErrors.NotFound(auctionId));

            var result = auction.Close(clock.UtcNow);
            if (result.IsFailure)
                return result;

            await uow.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
