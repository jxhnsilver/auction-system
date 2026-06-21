using AuctionSystem.Application.Features.Auctions.Commands.PlaceBid;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;

namespace AuctionSystem.Presentation.Endpoints.Auctions.Authenticated
{
    public sealed class PlaceBidEndpoint : IEndpoint
    {
        public sealed record Request(
            [Required]
            decimal Amount);

        public sealed record Response(Guid BidId);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/auctions/{id:guid}/bids", (Guid id, Request request, ISender sender, CancellationToken cancellationToken)
                    => PlaceBid(id, request, sender, cancellationToken))
                .RequireAuthorization()
                .WithTags("Auctions");
        }

        private static async Task<IResult> PlaceBid(Guid auctionId, Request request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new PlaceBidCommand(auctionId, request.Amount);

            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess)
                return Results.Created($"/api/auctions/{auctionId}/bids/{result.Value}", new Response(result.Value));

            return CustomResults.Problem(result);
        }
    }
}
