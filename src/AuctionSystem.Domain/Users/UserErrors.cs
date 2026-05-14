using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Users
{
    public static class UserErrors
    {
        public static Error NotFound(UserId userId) => Error.NotFound(
            $"The user with the Id = '{userId}' was not found");
        public static Error NotFound(Email email) => Error.NotFound(
            $"User with email '{email.Value}' was not found");
        public static Error InvalidCredentials() => Error.Validation(
            "Invalid credentials");
    }
}
