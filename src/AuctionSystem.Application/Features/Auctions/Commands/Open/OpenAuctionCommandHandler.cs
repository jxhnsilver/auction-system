using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Auctions;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Auctions.Commands.Open
{
    public sealed class OpenAuctionCommandHandler(
        IAuctionRepository auctionRepository,
        IClock clock,
        IUnitOfWork uow)
        : IRequestHandler<OpenAuctionCommand, Result>
    {
        public async Task<Result> Handle(OpenAuctionCommand request, CancellationToken cancellationToken)
        {
            if (!AuctionId.TryParse(request.AuctionId, out var auctionId))
                return Result.Failure(AuctionErrors.InvalidAuctionId());

            var auction = await auctionRepository.GetByIdAsync(auctionId, cancellationToken);
            if (auction is null)
                return Result.Failure(AuctionErrors.NotFound(auctionId));

            var result = auction.Open(clock.UtcNow);
            if (result.IsFailure)
                return result;

            await uow.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
