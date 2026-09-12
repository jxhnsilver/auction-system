using AuctionSystem.Application.Dtos;
using AuctionSystem.Application.Features.Lots.Queries.GetMy;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Presentation.Endpoints.Lots.Authenticated
{
    public sealed class GetMyLotsEndpoint : IEndpoint
    {
        public sealed record Response(IReadOnlyList<LotDto> Lots);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/me/lots", (ISender sender, CancellationToken cancellationToken)
                    => GetMyLots(sender, cancellationToken))
                .RequireAuthorization()
                .WithTags("Lots");
        }

        private static async Task<IResult> GetMyLots(ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetMyLotsQuery();

            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(new Response(result.Value));

            return CustomResults.Problem(result);
        }
    }
}