using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Security;

namespace AuctionSystem.Domain.Factories
{
    public sealed class AuctionFactory(
        IClock clock
        ) : IAuctionFactory
    {
        public Result<Auction> CreateNow(UserId sellerId, Lot lot, decimal startingPrice, DateTime endTime)
        {
            var lotValidation = ValidateLotForAuction(lot, sellerId);
            if (lotValidation.IsFailure)
                return Result<Auction>.Failure(lotValidation.Error);

            var now = clock.UtcNow;

            var inputValidation = ValidateNowInput(startingPrice, endTime, now);
            if (inputValidation.IsFailure)
                return Result<Auction>.Failure(inputValidation.Error);

            var auction = new Auction(
                AuctionId.New(), 
                sellerId, 
                lot.Id, 
                startingPrice, 
                now, 
                endTime, 
                AuctionStatus.Active);

            lot.MarkAsLocked();

            return Result<Auction>.Success(auction);
        }

        public Result<Auction> CreateScheduled(UserId sellerId, Lot lot, decimal startingPrice, DateTime startTime, DateTime endTime)
        {
            var now = clock.UtcNow;

            var lotValidation = ValidateLotForAuction(lot, sellerId);
            if (lotValidation.IsFailure)
                return Result<Auction>.Failure(lotValidation.Error);

            var inputValidation = ValidateScheduledInput(startingPrice, startTime, endTime, now);
            if (inputValidation.IsFailure)
                return Result<Auction>.Failure(inputValidation.Error);

            var auction = new Auction(
                AuctionId.New(), 
                sellerId, 
                lot.Id, 
                startingPrice, 
                startTime, 
                endTime, 
                AuctionStatus.Scheduled);

            lot.MarkAsLocked();

            return Result<Auction>.Success(auction);
        }

        private Result ValidateLotForAuction(Lot lot, UserId sellerId)
        {
            if (lot.OwnerId != sellerId)
                return Result.Failure(SecurityErrors.Forbidden());

            if (lot.Status != LotStatus.Available)
                return Result.Failure(LotErrors.InvalidStatus(lot.Status, LotStatus.Available));

            return Result.Success();
        }

        private Result ValidateNowInput(decimal startingPrice, DateTime endTime, DateTime now)
        {
            if (startingPrice <= 0)
                return Result.Failure(AuctionErrors.InvalidStartingPrice(startingPrice));

            if (endTime <= now)
                return Result.Failure(AuctionErrors.InvalidTimeRange(now, endTime));

            return Result.Success();
        }

        private Result ValidateScheduledInput(decimal startingPrice, DateTime startTime, DateTime endTime, DateTime now)
        {
            if (startingPrice <= 0)
                return Result.Failure(AuctionErrors.InvalidStartingPrice(startingPrice));

            if (endTime <= startTime)
                return Result.Failure(AuctionErrors.InvalidTimeRange(startTime, endTime));

            TimeSpan timeUntilStart = startTime - now;
            if (timeUntilStart < Auction.MinimumSchedulingBuffer)
            {
                var error = timeUntilStart < TimeSpan.Zero
                    ? AuctionErrors.StartTimeCannotBeInPast(startTime)
                    : AuctionErrors.StartTimeTooCloseToPresent(startTime, Auction.MinimumSchedulingBuffer);
                return Result.Failure(error);
            }

            return Result.Success();
        }
    }
}
