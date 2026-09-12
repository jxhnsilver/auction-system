using AuctionSystem.Application.Features.Users.Commands.Register;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;

namespace AuctionSystem.Presentation.Endpoints.Users.Public
{
    public sealed class RegisterEndpoint : IEndpoint
    {
        public sealed record Request(
            [Required, EmailAddress]
            string Email,
            [Required]
            string Password);

        public sealed record Response(Guid Id);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/users/register", (Request request, ISender sender, CancellationToken cancellationToken)
                => Register(request, sender, cancellationToken))
                .WithTags("Users");
        }

        private static async Task<IResult> Register(Request request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(request.Email, request.Password);

            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Created(
                    uri: $"/api/users/{result.Value}",
                    value: new Response(result.Value)
                );
            }

            return CustomResults.Problem(result);
        }
    }
}
