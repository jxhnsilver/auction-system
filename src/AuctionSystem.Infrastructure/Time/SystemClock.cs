using AuctionSystem.Application.Abstractions;

namespace AuctionSystem.Infrastructure.Time
{
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
