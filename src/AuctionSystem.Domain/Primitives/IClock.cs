namespace AuctionSystem.Domain.Primitives
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
