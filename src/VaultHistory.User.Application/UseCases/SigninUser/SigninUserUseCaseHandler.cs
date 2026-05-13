using MediatR;
using Microsoft.Extensions.Logging;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Providers.Jwt;
using VaultHistory.User.Application.Providers.PasswordHasher;
using VaultHistory.User.Application.Queries.GetUserByEmail;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.UseCases.SigninUser
{
    internal sealed class SigninUserUseCaseHandler(
        IMediator mediator,
        IPasswordHasherProvider passwordHasherProvider,
        IJwtProvider jwtProvider,
        ILogger<SigninUserUseCaseHandler> logger
    ) : IUseCaseHandler<SigninUserRequestDto, SigninUserResponseDto>
    {

        public async Task<Result<SigninUserResponseDto>> Handle(SigninUserRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;

            logger.LogInformation("Handling SigninUserUseCase for Email: {Email}", body.Email);

            var emailResult = Email.Create(body.Email);
            if (emailResult.IsFailure)
            {
                logger.LogWarning("Failed to parse Email '{Email}': {ErrorMessage}", body.Email, emailResult.Error.Message);
                return Result.Failure<SigninUserResponseDto>(emailResult.Error);
            }

            var getUserResult = await mediator.Send(new GetUserByEmailQuery(emailResult.Value), cancellationToken);
            if (getUserResult.IsFailure)
            {
                logger.LogWarning("Failed to retrieve user with Email '{Email}': {ErrorMessage}", body.Email, getUserResult.Error.Message);
                return Result.Failure<SigninUserResponseDto>(getUserResult.Error);
            }

            var user = getUserResult.Value;
            if (!user.IsActive)
            {
                logger.LogWarning("User with Email '{Email}' is inactive", body.Email);
                return Result.Failure<SigninUserResponseDto>(UserErrors.UserInactive);
            }

            var passwordVerificationResult = passwordHasherProvider.VerifyPassword(body.Password, user.Password.Hash, user.Password.Salt);
            if (passwordVerificationResult.IsFailure)
            {
                logger.LogWarning("Invalid password for Email '{Email}'", body.Email);
                return Result.Failure<SigninUserResponseDto>(UserErrors.InvalidPassword);
            }

            // Generate JWT token
            var token = jwtProvider.GenerateToken(user);

            logger.LogInformation("User with Email '{Email}' signed in successfully", body.Email);
            return Result.Success(new SigninUserResponseDto(token.Token, token.Expiration));
        }



    }
}