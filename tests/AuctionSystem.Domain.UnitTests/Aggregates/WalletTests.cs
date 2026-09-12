using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Aggregates.Wallets;
using System.Reflection;

namespace AuctionSystem.Domain.UnitTests.Aggregates
{
    public class WalletTests
    {
        private static readonly DateTime Now = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void Create_ShouldFail_WhenOwnerIsNull()
        {
            var result = Wallet.Create(null!);

            Assert.True(result.IsFailure);
            Assert.Equal(WalletErrors.InvalidOwnerId(), result.Error);
        }

        [Fact]
        public void Create_ShouldInitializeBalancesToZero()
        {
            var ownerId = UserId.New();

            var result = Wallet.Create(ownerId);

            Assert.True(result.IsSuccess);
            Assert.Equal(ownerId, result.Value.OwnerId);
            Assert.Equal(0m, result.Value.Balance);
            Assert.Equal(0m, result.Value.LockedBalance);
            Assert.Equal(0m, result.Value.AvailableBalance);
            Assert.Empty(result.Value.Holds);
        }

        [Fact]
        public void FreezeFunds_ShouldFail_WhenAmountIsNotPositive()
        {
            var wallet = Wallet.Create(UserId.New()).Value;
            wallet.Deposit(100m);

            var result = wallet.FreezeFunds(AuctionId.New(), 0m, Now);

            Assert.True(result.IsFailure);
            Assert.Equal(WalletErrors.InvalidAmount(0m), result.Error);
        }

        [Fact]
        public void FreezeFunds_ShouldFail_WhenBalanceIsInsufficient()
        {
            var wallet = Wallet.Create(UserId.New()).Value;
            wallet.Deposit(50m);

            var result = wallet.FreezeFunds(AuctionId.New(), 100m, Now);

            Assert.True(result.IsFailure);
            Assert.Equal(WalletErrors.InsufficientFunds(100m, 50m), result.Error);
        }

        [Fact]
        public void FreezeFunds_ShouldSucceed_AndIncreaseLockedBalance()
        {
            var wallet = Wallet.Create(UserId.New()).Value;
            wallet.Deposit(100m);
            var auctionId = AuctionId.New();

            var result = wallet.FreezeFunds(auctionId, 30m, Now);

            Assert.True(result.IsSuccess);
            Assert.Equal(100m, wallet.Balance);
            Assert.Equal(30m, wallet.LockedBalance);
            Assert.Equal(70m, wallet.AvailableBalance);
            Assert.Single(wallet.Holds);
        }

        [Fact]
        public void FreezeFunds_ShouldFail_WhenActiveHoldAlreadyExists()
        {
            var wallet = Wallet.Create(UserId.New()).Value;
            wallet.Deposit(100m);

            var auctionId = AuctionId.New();

            Assert.True(wallet.FreezeFunds(auctionId, 30m, Now).IsSuccess);

            var result = wallet.FreezeFunds(auctionId, 20m, Now.AddMinutes(1));

            Assert.True(result.IsFailure);
            Assert.Equal(WalletErrors.ActiveHoldAlreadyExists(auctionId), result.Error);
        }

        [Fact]
        public void UnfreezeFunds_ShouldSucceed_WhenActiveHoldExists()
        {
            var wallet = Wallet.Create(UserId.New()).Value;
            wallet.Deposit(100m);

            var auctionId = AuctionId.New();

            Assert.True(wallet.FreezeFunds(auctionId, 30m, Now).IsSuccess);

            var result = wallet.UnfreezeFunds(auctionId, Now.AddMinutes(1));

            Assert.True(result.IsSuccess);
            Assert.Equal(0m, wallet.LockedBalance);
            Assert.Equal(100m, wallet.AvailableBalance);
            Assert.Single(wallet.Holds);
            Assert.Equal(WalletHoldStatus.Released, wallet.Holds.Single().Status);
        }

        [Fact]
        public void CaptureFunds_ShouldSucceed_WhenActiveHoldExists()
        {
            var wallet = Wallet.Create(UserId.New()).Value;
            wallet.Deposit(100m);

            var auctionId = AuctionId.New();

            Assert.True(wallet.FreezeFunds(auctionId, 30m, Now).IsSuccess);

            var result = wallet.CaptureFunds(auctionId, Now.AddMinutes(1));

            Assert.True(result.IsSuccess);
            Assert.Equal(70m, wallet.Balance);
            Assert.Equal(0m, wallet.LockedBalance);
            Assert.Equal(70m, wallet.AvailableBalance);
            Assert.Equal(WalletHoldStatus.Captured, wallet.Holds.Single().Status);
        }

        [Fact]
        public void UnfreezeFunds_ShouldFail_WhenNoActiveHoldExists()
        {
            var wallet = Wallet.Create(UserId.New()).Value;
            wallet.Deposit(100m);

            var auctionId = AuctionId.New();

            var result = wallet.UnfreezeFunds(auctionId, Now);

            Assert.True(result.IsFailure);
            Assert.Equal(WalletErrors.NoActiveHoldForAuction(auctionId), result.Error);
        }
    }
}
