using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Aggregates.Users.Errors;
using AuctionSystem.Domain.Aggregates.Wallets;
using AuctionSystem.Domain.Primitives;
using MediatR;

namespace AuctionSystem.Application.Features.Users.Commands.Register
{
    public class RegisterUserCommandHandler(
        IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IWalletRepository walletRepository,
        IUnitOfWork uow)
        : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            Result<Email> emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
                return Result<Guid>.Failure(emailResult.Error);

            if (await userRepository.ExistsByEmailAsync(emailResult.Value, cancellationToken))
                return Result<Guid>.Failure(UserErrors.EmailNotUnique);

            var passwordHash = passwordHasher.Hash(request.Password);

            var userId = UserId.New();

            var user = User.Create(userId, emailResult.Value, passwordHash);
            var userWallet = Wallet.Create(userId).Value;

            userRepository.Add(user);
            walletRepository.Add(userWallet);

            await uow.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id.Value);
        }
    }
}
