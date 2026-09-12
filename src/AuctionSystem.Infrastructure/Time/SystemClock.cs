using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Infrastructure.Time
{
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
