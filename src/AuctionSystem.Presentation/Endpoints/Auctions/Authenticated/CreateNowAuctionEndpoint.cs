using AuctionSystem.Application.Features.Auctions.Commands.CreateNow;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;

namespace AuctionSystem.Presentation.Endpoints.Auctions.Authenticated
{
    public sealed class CreateNowAuctionEndpoint : IEndpoint
    {
        public sealed record Request(
            [Required]
            Guid LotId,
            [Required]
            decimal StartingPrice,
            [Required]
            DateTime EndTime);

        public sealed record Response(Guid Id);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/auctions/now", (Request request, ISender sender, CancellationToken cancellationToken)
                    => Create(request, sender, cancellationToken))
            .RequireAuthorization()
            .WithTags("Auctions");
        }

        private static async Task<IResult> Create(Request request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new CreateNowAuctionCommand(request.LotId, request.StartingPrice, request.EndTime);

            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess)
                return Results.Created($"/api/auctions/{result.Value}", new Response(result.Value));

            return CustomResults.Problem(result);
        }
    }
}
