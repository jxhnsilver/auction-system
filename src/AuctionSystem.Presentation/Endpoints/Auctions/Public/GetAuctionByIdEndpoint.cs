using AuctionSystem.Application.Dtos;
using AuctionSystem.Application.Features.Auctions.Queries.GetById;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Presentation.Endpoints.Auctions.Public
{
    public sealed class GetAuctionByIdEndpoint : IEndpoint
    {
        public sealed record Response(AuctionDto Auction);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/auctions/{id:guid}", (Guid id, ISender sender, CancellationToken cancellationToken)
                    => GetById(id, sender, cancellationToken))
                .WithTags("Auctions");
        }

        private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetAuctionByIdQuery(id);

            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(new Response(result.Value));

            return CustomResults.Problem(result);
        }
    }
}