using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Aggregates.Wallets;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuctionSystem.Application.Features.Auctions.Commands.PlaceBid
{
    public sealed class PlaceBidCommandHandler(
        ICurrentUserProvider currentUserProvider,
        IAuctionRepository auctionRepository,
        IWalletRepository walletRepository,
        IClock clock,
        IUnitOfWork uow,
        ILogger<PlaceBidCommandHandler> logger)
        : IRequestHandler<PlaceBidCommand, Result>
    {
        private const int MaxConcurrencyAttempts = 3;

        public async Task<Result> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
        {
            if (!AuctionId.TryParse(request.AuctionId, out var auctionId))
                return Result.Failure(AuctionErrors.InvalidAuctionId());

            var currentBidderId = currentUserProvider.UserId;
            if (currentBidderId is null)
                return Result.Failure(SecurityErrors.Unauthorized());

            var now = clock.UtcNow;

            for (int attempt = 1; attempt <= MaxConcurrencyAttempts; attempt++)
            {
                await using var transaction = await uow.BeginTransactionAsync(cancellationToken);
                try
                {
                    var result = await ProcessPlaceBidAsync(
                        auctionId, 
                        currentBidderId, 
                        request.Amount, 
                        now, 
                        cancellationToken);

                    if (result.IsFailure)
                    {
                        await transaction.RollbackAsync();
                        return result;
                    }

                    await uow.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync();

                    logger.LogInformation(
                            "Bid placed successfully for auction {AuctionId} by user {UserId}. Amount: {Amount}",
                            auctionId.Value,
                            currentBidderId.Value,
                            request.Amount);

                    return Result.Success();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync();

                    logger.LogWarning(
                            ex,
                            "Concurrency conflict while user {UserId} placing bid for auction {AuctionId}",
                            currentBidderId.Value,
                            auctionId.Value);

                    if (attempt == MaxConcurrencyAttempts)
                        return Result.Failure(Error.Conflict("The auction was updated by another process. Please try again"));
                }
                catch (Exception ex) 
                {
                    await transaction.RollbackAsync();

                    logger.LogError(ex, "Unexpected error while placing bid for auction {AuctionId}", auctionId.Value);

                    throw;
                }
            }

            return Result.Success();
        }

        private async Task<Result> ProcessPlaceBidAsync(
            AuctionId auctionId, 
            UserId currentBidderId,
            decimal amount,
            DateTime now,
            CancellationToken cancellationToken
            )
        {
            var auction = await auctionRepository.GetByIdAsync(auctionId, cancellationToken);
            if (auction is null)
                return Result.Failure(AuctionErrors.NotFound(auctionId));

            var previousBidderId = auction.LastBidderId;

            var bidResult = auction.PlaceBid(currentBidderId, amount, now);
            if (bidResult.IsFailure)
                return bidResult;

            var currentBidderWallet = await walletRepository.GetByOwnerIdWithActiveHoldAsync(
                currentBidderId,
                auction.Id,
                cancellationToken);

            var freezeResult = currentBidderWallet.FreezeFunds(auction.Id, amount, now);
            if (freezeResult.IsFailure)
                return freezeResult;

            if (previousBidderId is not null && previousBidderId != currentBidderId)
            {
                var previousBidderWallet = await walletRepository.GetByOwnerIdWithActiveHoldAsync(
                    previousBidderId,
                    auction.Id,
                    cancellationToken);

                var unfreezeResult = previousBidderWallet.UnfreezeFunds(auction.Id, now);
                if (unfreezeResult.IsFailure)
                    return unfreezeResult;
            }

            return Result.Success();
        }
    }
}
