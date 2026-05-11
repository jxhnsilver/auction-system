using AuctionSystem.Domain.Primitives;

namespace AuctionSystem.Domain.Users
{
    public class User : AggregateRoot<UserId>
    {
        public Email Email { get; private set; }
        public PasswordHash PasswordHash { get; private set; }

        private User(UserId id, Email email, PasswordHash passwordHash) : base(id)
        {
            Email = email;
            PasswordHash = passwordHash;
        }

        /// <summary>
        /// Parameterless constructor required by EF Core
        /// </summary>
        private User() { }

        public static User Create(UserId id, Email email, PasswordHash passwordHash) 
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            if (email is null) 
                throw new ArgumentNullException(nameof(email));
            if (passwordHash is null) 
                throw new ArgumentNullException(nameof(passwordHash));

            return new User(id, email, passwordHash);
        }
    }
}
