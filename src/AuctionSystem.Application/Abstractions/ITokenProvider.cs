using AuctionSystem.Domain.Aggregates.Users;

namespace AuctionSystem.Application.Abstractions
{
    public interface ITokenProvider
    {
        string GenerateAccessToken(User user);
    }
}
