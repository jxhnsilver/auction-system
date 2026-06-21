using AuctionSystem.Application.Features.Auctions.Commands.CreateScheduled;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;

namespace AuctionSystem.Presentation.Endpoints.Auctions.Authenticated
{
    public sealed class CreateScheduledAuctionEndpoint : IEndpoint
    {
        public sealed record Request(
            [Required]
            Guid LotId,
            [Required]
            decimal StartingPrice,
            [Required]
            DateTime StartTime,
            [Required]
            DateTime EndTime);

        public sealed record Response(Guid Id);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/auctions/scheduled", (Request request, ISender sender, CancellationToken cancellationToken)
                    => Create(request, sender, cancellationToken))
            .RequireAuthorization()
            .WithTags("Auctions");
        }

        private static async Task<IResult> Create(Request request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new CreateScheduledAuctionCommand(request.LotId, request.StartingPrice, request.StartTime, request.EndTime);

            var result = await sender.Send(command, cancellationToken);
            if (result.IsSuccess)
                return Results.Created($"/api/auctions/{result.Value}", new Response(result.Value));

            return CustomResults.Problem(result);
        }
    }
}
