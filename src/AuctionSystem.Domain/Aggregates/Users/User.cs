using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Aggregates.Users
{
    public sealed class User : AggregateRoot<UserId>
    {
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }

        private User(UserId id, Email email, string passwordHash) : base(id)
        {
            Email = email;
            PasswordHash = passwordHash;
        }

        // Parameterless constructor required by EF Core
        private User() { }

        public static User Create(UserId id, Email email, string passwordHash) 
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (email is null) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

            return new User(id, email, passwordHash);
        }
    }
}
