using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Users.Errors
{
    public static class EmailErrors
    {
        public static readonly Error Empty = Error.Validation("Email is empty");
        public static readonly Error InvalidFormat = Error.Validation("Email format is invalid");
    }
}
