using AuctionSystem.Domain.Users;

namespace AuctionSystem.Application.Abstractions
{
    public interface ICurrentUserProvider
    {
        public UserId? UserId { get; }
    }
}
