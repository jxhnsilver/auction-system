namespace AuctionSystem.Domain.Primitives
{
    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        protected AggregateRoot(TId id) : base(id) { }

        /// <summary>
        /// Parameterless constructor required by EF Core
        /// </summary>
        protected AggregateRoot() { }
    }
}
