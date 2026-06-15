using AuctionSystem.Application.Features.Lots.Commands.Create;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;

namespace AuctionSystem.Presentation.Endpoints.Lots.Me
{
    public sealed class CreateLotEndpoint : IEndpoint
    {
        public sealed record Request(
            [Required]
            string Title);

        public sealed record Response(Guid Id);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/me/lots", (Request request, ISender sender, CancellationToken cancellationToken)
                    => Create(request, sender, cancellationToken))
                .RequireAuthorization();
        }

        private static async Task<IResult> Create(Request request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new CreateLotCommand(request.Title);

            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.Created($"/api/me/lots/{result.Value}", new Response(result.Value));

            return CustomResults.Problem(result);
        }
    }
}