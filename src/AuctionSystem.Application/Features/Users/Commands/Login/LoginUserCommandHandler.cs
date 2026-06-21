using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Users;
using AuctionSystem.Domain.Users.Errors;
using MediatR;

namespace AuctionSystem.Application.Features.Users.Commands.Login
{
    public class LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider
        ) 
        : IRequestHandler<LoginUserCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            Result<Email> emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
                return Result<string>.Failure(emailResult.Error);

            User? user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
            if (user is null)
                return Result<string>.Failure(UserErrors.InvalidCredentials);

            bool isPasswordValid = passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Result<string>.Failure(UserErrors.InvalidCredentials);

            var token = tokenProvider.GenerateAccessToken(user);

            return Result<string>.Success(token);
        }
    }
}
