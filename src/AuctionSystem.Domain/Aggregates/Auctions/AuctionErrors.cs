using AuctionSystem.Domain.Primitives;
using System.Diagnostics;

namespace AuctionSystem.Domain.Aggregates.Auctions
{
    public static class AuctionErrors
    {
        public static Error InvalidAuctionId() => Error.Validation("Invalid auction id");
        public static Error NotFound(AuctionId auctionId) => Error.NotFound(
            $"Auction with id '{auctionId.Value}' was not found");
        public static Error InvalidStatus(AuctionStatus current, AuctionStatus expected) => Error.Conflict(
            $"Invalid auction status. Current: {current}. Expected: {expected}");
        public static Error InvalidStartingPrice(decimal startingPrice) => Error.Validation(
            $"Starting price must be greater than zero. Actual: {startingPrice}");
        public static Error SellerCannotBid() => Error.Conflict(
            "Seller cannot place a bid on their own auction");
        public static Error MustMeetStartingPrice(decimal amount, decimal startingPrice) => Error.Conflict(
            $"Bid must be at least the starting price. Amount: {amount}, Starting price: {startingPrice}");
        public static Error MustExceedCurrentPrice(decimal amount, decimal currentPrice) => Error.Conflict(
            $"Bid must exceed the current price. Amount: {amount}, Current price: {currentPrice}");
        public static Error InvalidTimeRange(DateTime startTime, DateTime endTime) => Error.Validation(
            $"Auction end time must be greater than start time. Start: {startTime}, End: {endTime}");
        public static Error NotStarted(DateTime startTime, DateTime now) => Error.Conflict(
            $"Auction has not started yet. Start: {startTime}, Now: {now}");
        public static Error NotInBiddingWindow(DateTime now, DateTime startTime, DateTime endTime) => Error.Conflict(
            $"Auction is not in bidding window. Now: {now}, Start: {startTime}, End: {endTime}");
        public static Error CannotCloseBeforeEnd(DateTime now, DateTime endTime) => Error.Conflict(
            $"Auction cannot be closed before end time. Now: {now}, End: {endTime}");
        public static Error StartTimeCannotBeInPast(DateTime startTime) => Error.Validation(
            $"Start time '{startTime}' cannot be in the past");
        public static Error StartTimeTooCloseToPresent(DateTime startTime, TimeSpan buffer) => Error.Validation(
            $"Start time '{startTime}' is too close to the present. Please schedule at least {buffer.TotalSeconds} seconds into the future");
        public static Error AlreadyClosed() => Error.Conflict("Cannot cancel an auction that is already closed");
        public static Error CannotCancelWithBids() => Error.Conflict("Cannot cancel an auction that has bids");
        public static Error AlreadyCancelled() => Error.Conflict("Cannot cancel an auction that is already cancelled");
    }
}
