using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Factories;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.UnitTests.Fakes;
using AuctionSystem.Infrastructure.Time;
using System.Reflection;
using Xunit;

namespace AuctionSystem.Domain.UnitTests.Aggregates;

public class AuctionTests
{
    private static readonly DateTime StartTime = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime EndTime = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private readonly FakeClock _clock = new(StartTime);
    private readonly IAuctionFactory _factory;

    private readonly UserId sellerId = UserId.New();

    public AuctionTests()
    {
        _factory = new AuctionFactory(_clock);
    }

    private Auction CreateAuction(
        AuctionStatus status = AuctionStatus.Active,
        decimal startingPrice = 100m)
    {
        var lot = Lot.Create(LotId.New(), sellerId, "Item");

        var result = status == AuctionStatus.Active
            ? _factory.CreateNow(sellerId, lot, startingPrice, EndTime)
            : _factory.CreateScheduled(sellerId, lot, startingPrice, StartTime.AddHours(1), EndTime);

        Assert.True(result.IsSuccess);
        return result.Value;
    }

    [Theory]
    [InlineData(AuctionStatus.Closed)]
    [InlineData(AuctionStatus.Cancelled)]
    [InlineData(AuctionStatus.Scheduled)]
    public void PlaceBid_ShouldFail_WhenAuctionIsNotActive(AuctionStatus status)
    {
        var auction = CreateAuction(status);

        var result = auction.PlaceBid(UserId.New(), 120m, StartTime.AddMinutes(10));

        Assert.True(result.IsFailure);
        Assert.Equal(AuctionErrors.InvalidStatus(AuctionStatus.Scheduled, AuctionStatus.Active), result.Error);
    }

    [Fact]
    public void PlaceBid_ShouldFail_WhenBidderIsSeller()
    {
        var auction = CreateAuction();

        var result = auction.PlaceBid(sellerId, 120m, StartTime.AddMinutes(10));

        Assert.True(result.IsFailure);
        Assert.Equal(AuctionErrors.SellerCannotBid(), result.Error);
    }

    [Fact]
    public void PlaceBid_ShouldFail_WhenOutsideBiddingWindow()
    {
        var auction = CreateAuction();

        var result = auction.PlaceBid(UserId.New(), 120m, EndTime.AddMinutes(1));

        Assert.True(result.IsFailure);
        Assert.Equal(AuctionErrors.NotInBiddingWindow(EndTime.AddMinutes(1), StartTime, EndTime), result.Error);
    }

    [Fact]
    public void PlaceBid_ShouldFail_WhenFirstBidIsBelowStartingPrice()
    {
        var auction = CreateAuction(startingPrice: 100m);

        var result = auction.PlaceBid(UserId.New(), 99m, StartTime.AddMinutes(10));

        Assert.True(result.IsFailure);
        Assert.Equal(AuctionErrors.MustMeetStartingPrice(99m, 100m), result.Error);
    }

    [Fact]
    public void PlaceBid_ShouldSucceed_AndUpdateAuctionState()
    {
        var bidderId = UserId.New();
        var auction = CreateAuction(startingPrice: 100m);

        var result = auction.PlaceBid(bidderId, 120m, StartTime.AddMinutes(10));

        Assert.True(result.IsSuccess);
        Assert.Equal(120m, auction.CurrentPrice);
        Assert.Equal(bidderId, auction.LastBidderId);
        Assert.Single(auction.Bids);
    }

    [Fact]
    public void PlaceBid_ShouldFail_WhenSameBidderTriesToOutbidAgain()
    {
        var bidderId = UserId.New();
        var auction = CreateAuction();

        var first = auction.PlaceBid(bidderId, 120m, StartTime.AddMinutes(10));
        Assert.True(first.IsSuccess);

        var second = auction.PlaceBid(bidderId, 140m, StartTime.AddMinutes(11));

        Assert.True(second.IsFailure);
        Assert.Equal(AuctionErrors.CannotOutbidYourself(), second.Error);
        Assert.Single(auction.Bids);
        Assert.Equal(120m, auction.CurrentPrice);
    }

    [Fact]
    public void PlaceBid_ShouldFail_WhenNextBidDoesNotExceedCurrentPrice()
    {
        var auction = CreateAuction();

        var first = auction.PlaceBid(UserId.New(), 120m, StartTime.AddMinutes(10));
        Assert.True(first.IsSuccess);

        var second = auction.PlaceBid(UserId.New(), 120m, StartTime.AddMinutes(11));

        Assert.True(second.IsFailure);
        Assert.Equal(AuctionErrors.MustExceedCurrentPrice(120m, 120m), second.Error);
    }

    [Fact]
    public void Cancel_ShouldSucceed_WhenThereAreNoBids()
    {
        var auction = CreateAuction(status: AuctionStatus.Active);

        var result = auction.Cancel(StartTime.AddMinutes(20));

        Assert.True(result.IsSuccess);
        Assert.Equal(AuctionStatus.Cancelled, auction.Status);
    }

    [Fact]
    public void Cancel_ShouldFail_WhenAuctionHasBids()
    {
        var auction = CreateAuction();
        var bidResult = auction.PlaceBid(UserId.New(), 120m, StartTime.AddMinutes(10));
        Assert.True(bidResult.IsSuccess);

        var result = auction.Cancel(StartTime.AddMinutes(20));

        Assert.True(result.IsFailure);
        Assert.Equal(AuctionErrors.CannotCancelWithBids(), result.Error);
    }

    [Fact]
    public void Cancel_ShouldFail_WhenAuctionAlreadyCancelled()
    {
        var auction = CreateAuction();
        var first = auction.Cancel(StartTime.AddMinutes(20));
        Assert.True(first.IsSuccess);

        var second = auction.Cancel(StartTime.AddMinutes(21));

        Assert.True(second.IsFailure);
        Assert.Equal(AuctionErrors.AlreadyCancelled(), second.Error);
    }

    [Fact]
    public void Cancel_ShouldFail_WhenAuctionAlreadyClosedByTime()
    {
        var auction = CreateAuction();

        var result = auction.Cancel(EndTime.AddMinutes(1));

        Assert.True(result.IsFailure);
        Assert.Equal(AuctionErrors.AlreadyClosed(), result.Error);
    }
}