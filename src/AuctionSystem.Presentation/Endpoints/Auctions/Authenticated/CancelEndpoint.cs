using AuctionSystem.Application.Features.Auctions.Commands.Cancel;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Presentation.Endpoints.Auctions.Authenticated
{
    public sealed class CancelEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/auctions/{id:guid}/cancel", (Guid id, ISender sender, CancellationToken cancellationToken)
                => Cancel(id, sender, cancellationToken))
                .RequireAuthorization()
                .WithTags("Auctions");
        }

        private static async Task<IResult> Cancel(Guid id, ISender sender, CancellationToken cancellationToken)
        {
            var command = new CancelAuctionCommand(id);

            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess)
                return Results.Ok();

            return CustomResults.Problem(result);
        }
    }
}
