using AuctionSystem.Application.Dtos;
using AuctionSystem.Application.Features.Lots.Queries.GetById;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Presentation.Endpoints.Lots.Public
{
    public sealed class GetLotByIdEndpoint : IEndpoint
    {
        public sealed record Response(LotDto Lot);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/lots/{id:guid}", (Guid id, ISender sender, CancellationToken cancellationToken)
                    => GetById(id, sender, cancellationToken))
                .WithTags("Lots");
        }

        private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken cancellationToken)
        {
            var query = new GetLotByIdQuery(id);

            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(new Response(result.Value));

            return CustomResults.Problem(result);
        }
    }
}