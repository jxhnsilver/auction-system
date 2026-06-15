using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Lots;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Security;
using MediatR;

namespace AuctionSystem.Application.Features.Lots.Commands.Create
{
    public class CreateLotCommandHandler(
        ICurrentUserProvider currentUserProvider,
        ILotRepository lotRepository,
        IUnitOfWork uow)
        : IRequestHandler<CreateLotCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateLotCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserProvider.UserId;
            if (userId is null)
                return Result<Guid>.Failure(SecurityErrors.Unauthorized());

            var lot = Lot.Create(LotId.New(), userId, request.Title);

            lotRepository.Add(lot);

            await uow.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(lot.Id.Value);
        }
    }
}