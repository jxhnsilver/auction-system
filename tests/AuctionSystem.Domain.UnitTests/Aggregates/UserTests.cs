using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Aggregates.Users.Errors;

namespace AuctionSystem.Domain.UnitTests.Aggregates
{
    public class UserTests
    {
        [Fact]
        public void Create_ShouldThrow_WhenIdIsNull()
        {
            var email = Email.Create("user@example.com").Value;

            Assert.Throws<ArgumentNullException>(() =>
                User.Create(null!, email, "hash"));
        }

        [Fact]
        public void Create_ShouldThrow_WhenEmailIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                User.Create(UserId.New(), null!, "hash"));
        }

        [Fact]
        public void Create_ShouldThrow_WhenPasswordHashIsEmpty()
        {
            var email = Email.Create("user@example.com").Value;

            Assert.Throws<ArgumentException>(() =>
                User.Create(UserId.New(), email, ""));
        }

        [Fact]
        public void Create_ShouldCreateUser()
        {
            var id = UserId.New();
            var email = Email.Create("user@example.com").Value;

            var user = User.Create(id, email, "hash");

            Assert.Equal(id, user.Id);
            Assert.Equal(email, user.Email);
            Assert.Equal("hash", user.PasswordHash);
        }

        [Fact]
        public void Email_Create_ShouldNormalizeAndValidate()
        {
            var result = Email.Create("  User@Example.com  ");

            Assert.True(result.IsSuccess);
            Assert.Equal("user@example.com", result.Value.Value);
        }

        [Fact]
        public void Email_Create_ShouldFail_WhenEmpty()
        {
            var result = Email.Create("");

            Assert.True(result.IsFailure);
            Assert.Equal(EmailErrors.Empty, result.Error);
        }
    }
}
