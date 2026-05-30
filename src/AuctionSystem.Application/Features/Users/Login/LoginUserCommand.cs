using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Users.Login
{
    public sealed record LoginUserCommand(
        string Email,
        string Password
        ) : IRequest<Result<string>>;
}
