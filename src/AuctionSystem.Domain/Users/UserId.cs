namespace AuctionSystem.Domain.Users
{
    public record UserId
    {
        public Guid Value { get; }

        private UserId(Guid value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new unique user identifier.
        /// </summary>
        public static UserId New() => new UserId(Guid.NewGuid());

        /// <summary>
        /// Creates an user identifier from an existing GUID value.
        /// </summary>
        /// <param name="userId">The GUID value.</param>
        /// <exception cref="ArgumentException">Thrown when the GUID is empty.</exception>
        public static UserId From(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return new UserId(userId);
        }
    }
}
