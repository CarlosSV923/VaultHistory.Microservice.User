using MediatR;
using Microsoft.Extensions.Logging;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Commands.CreateUser;
using VaultHistory.User.Application.Providers.Jwt;
using VaultHistory.User.Application.Providers.PasswordHasher;
using VaultHistory.User.Application.Queries.VerifyUserEmailExists;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.ValueObjects;

namespace VaultHistory.User.Application.UseCases.SignupUser
{
    internal sealed class SignupUserUseCaseHandler(
        IMediator mediator,
        IPasswordHasherProvider passwordHasherProvider,
        IJwtProvider jwtProvider,
        ILogger<SignupUserUseCaseHandler> logger
    ) : IUseCaseHandler<SignupUserRequestDto, SignupUserResponseDto>
    {


        public async Task<Result<SignupUserResponseDto>> Handle(SignupUserRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;

            logger.LogInformation("Handling SignupUserUseCase for Email: {Email}", body.Email);

            var emailResult = Email.Create(body.Email);

            if (emailResult.IsFailure)
            {
                logger.LogWarning("Failed to parse Email '{Email}': {ErrorMessage}", body.Email, emailResult.Error.Message);
                return Result.Failure<SignupUserResponseDto>(emailResult.Error);
            }

            // Check if user with the same email already exists
            var existingUserResult = await mediator.Send(new VerifyUserEmailExistsQuery(emailResult.Value), cancellationToken);
            if (existingUserResult.IsSucceeded)
            {
                logger.LogWarning("User with Email '{Email}' already exists", body.Email);
                return Result.Failure<SignupUserResponseDto>(UserErrors.UserAlreadyExists);
            }

            // validate password
            var validatePasswordResult = passwordHasherProvider.ValidatePassword(body.Password);
            if (validatePasswordResult.IsFailure)
            {
                logger.LogWarning("Password validation failed for Email '{Email}': {ErrorMessage}", body.Email, validatePasswordResult.Error.Message);
                return Result.Failure<SignupUserResponseDto>(validatePasswordResult.Error);
            }

            // Hash the password
            var passwordHashResult = passwordHasherProvider.HashPassword(body.Password);
            var passwordCreate = Password.Create(passwordHashResult.Hash, passwordHashResult.Salt);
            if (passwordCreate.IsFailure)
            {
                logger.LogWarning("Failed to create password for Email '{Email}': {ErrorMessage}", body.Email, passwordCreate.Error.Message);
                return Result.Failure<SignupUserResponseDto>(passwordCreate.Error);
            }

            var fullNameCreateResult = FullName.Create(body.FirstName, body.LastName);

            if (fullNameCreateResult.IsFailure)
            {
                logger.LogWarning("Failed to create FullName for Email '{Email}': {ErrorMessage}", body.Email, fullNameCreateResult.Error.Message);
                return Result.Failure<SignupUserResponseDto>(fullNameCreateResult.Error);
            }


            // Create new user
            var user = Domain.Users.User.Create(
                new CreateUserData(
                    fullNameCreateResult.Value,
                    emailResult.Value,
                    passwordCreate.Value,
                    body.DateOfBirth,
                    body.Notification,
                    body.Theme,
                    body.Character
                )
            );

            if (user.IsFailure)
            {
                logger.LogWarning("Failed to create user for Email '{Email}': {ErrorMessage}", body.Email, user.Error.Message);
                return Result.Failure<SignupUserResponseDto>(user.Error);
            }

            // Save user to repository
            await mediator.Send(new CreateUserCommand(user.Value), cancellationToken);

            // Generate JWT token
            var token = jwtProvider.GenerateToken(user.Value);
            logger.LogInformation("User with Email '{Email}' signed up successfully", body.Email); 
            return Result.Success(new SignupUserResponseDto(token.Token, token.Expiration));
        }
    }
}
