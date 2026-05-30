using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Users;
using Microsoft.AspNetCore.Http;

namespace AuctionSystem.Infrastructure.Authentication
{
    public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
    {
        public UserId? UserId
        {
            get
            {
                var user = httpContextAccessor.HttpContext?.User;
                var userIdClaim = user?.FindFirst("sub")?.Value;

                if (Guid.TryParse(userIdClaim, out var parsedGuid) && parsedGuid != Guid.Empty)
                    return UserId.From(parsedGuid);

                return null;
            }
        }
    }
}
