using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Security
{
    public static class SecurityErrors
    {
        public static Error Unauthorized() => Error.Unauthorized("User is not authenticated");
    }
}
