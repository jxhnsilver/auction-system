using AuctionSystem.Application.Dtos;
using AuctionSystem.Application.Features.Auctions.Queries.GetAll;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Presentation.Endpoints.Auctions.Public
{
    public sealed class GetAuctionsEndpoint : IEndpoint
    {
        public sealed record Response(IReadOnlyList<AuctionSummaryDto> Auctions);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/auctions", (ISender sender, CancellationToken cancellationToken)
                    => GetAll(sender, cancellationToken))
                .WithTags("Auctions");
        }

        private static async Task<IResult> GetAll(ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetAuctionsQuery();

            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(new Response(result.Value));

            return CustomResults.Problem(result);
        }
    }
}