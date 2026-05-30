namespace AuctionSystem.Domain.Primitives
{
    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        protected AggregateRoot(TId id) : base(id) { }

        // Parameterless constructor required by EF Core
        protected AggregateRoot() { }
    }
}
