using AuctionSystem.Application.Features.Users.Commands.Login;
using AuctionSystem.Presentation.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;

namespace AuctionSystem.Presentation.Endpoints.Users
{
    public sealed class LoginEndpoint : IEndpoint
    {
        public sealed record Request(
            [Required, EmailAddress]
            string Email,
            [Required]
            string Password);

        public sealed record Response(string Token);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/users/login", (Request request, ISender sender, CancellationToken cancellationToken)
                => Login(request, sender, cancellationToken))
                .WithTags("Users");
        }

        private static async Task<IResult> Login(Request request, ISender sender, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(request.Email, request.Password);

            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(new Response(Token: result.Value));

            return CustomResults.Problem(result);
        }
    }
}
