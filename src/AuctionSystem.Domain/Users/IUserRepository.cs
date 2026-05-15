namespace AuctionSystem.Domain.Users
{
    public interface IUserRepository
    {
        void Add(User user);
        Task<User?> GetById(UserId userId, CancellationToken cancellationToken);
        Task<User?> GetByEmail(Email email, CancellationToken cancellationToken);
    }
}
