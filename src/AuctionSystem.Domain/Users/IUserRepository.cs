namespace AuctionSystem.Domain.Users
{
    public interface IUserRepository
    {
        void Add(User user);
        Task<User?> GetByIdAsync(UserId userId, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken);
    }
}
