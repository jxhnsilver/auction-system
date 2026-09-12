using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.UnitTests.Fakes
{
    public sealed class FakeClock : IClock
    {
        public DateTime UtcNow { get; set; }

        public FakeClock(DateTime utcNow)
        {
            UtcNow = utcNow;
        }
    }
}
