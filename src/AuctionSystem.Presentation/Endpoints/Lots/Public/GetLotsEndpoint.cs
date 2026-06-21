using AuctionSystem.Application.Dtos;
using AuctionSystem.Application.Features.Lots.Queries.GetAll;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Presentation.Endpoints.Lots.Public
{
    public sealed class GetLotsEndpoint : IEndpoint
    {
        public sealed record Response(IReadOnlyList<LotDto> Lots);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/lots", (ISender sender, CancellationToken cancellationToken)
                    => GetAll(sender, cancellationToken))
                .WithTags("Lots");
        }

        private static async Task<IResult> GetAll(ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetLotsQuery();

            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(new Response(result.Value));

            return CustomResults.Problem(result);
        }
    }
}