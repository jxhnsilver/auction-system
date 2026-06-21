using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Users.Commands.Register
{
    public sealed record RegisterUserCommand(
        string Email,
        string Password
        ) : IRequest<Result<Guid>>;
}
