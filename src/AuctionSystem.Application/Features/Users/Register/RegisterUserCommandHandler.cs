using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Users;
using AuctionSystem.Domain.Users.Errors;
using MediatR;

namespace AuctionSystem.Application.Features.Users.Register
{
    public class RegisterUserCommandHandler(
        IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IUnitOfWork uow)
        : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            Result<Email> emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
                return Result<Guid>.Failure(emailResult.Error);

            var existingUser = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
            if (existingUser is not null)
                return Result<Guid>.Failure(UserErrors.EmailNotUnique());

            var passwordHash = passwordHasher.Hash(request.Password);

            var user = User.Create(UserId.New(), emailResult.Value, passwordHash);

            userRepository.Add(user);

            await uow.SaveChangesAsync();

            return Result<Guid>.Success(user.Id.Value);
        }
    }
}
