using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Presentation.Endpoints
{
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}
