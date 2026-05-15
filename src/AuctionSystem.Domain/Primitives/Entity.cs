namespace AuctionSystem.Domain.Primitives
{
    public abstract class Entity<TId> where TId : notnull
    {
        public TId Id { get; protected set; }

        protected Entity(TId id)
        {
            Id = id;
        }

        /// <summary>
        /// Parameterless constructor required by EF Core
        /// </summary>
        protected Entity() { }
    }
}
