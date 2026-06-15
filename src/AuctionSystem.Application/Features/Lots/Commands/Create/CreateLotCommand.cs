using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Lots.Commands.Create
{
    public sealed record CreateLotCommand(string Title) : IRequest<Result<Guid>>;
}