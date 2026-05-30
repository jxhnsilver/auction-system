using AuctionSystem.Domain.Users;

namespace AuctionSystem.Application.Abstractions
{
    public interface ITokenProvider
    {
        string GenerateAccessToken(User user);
    }
}
