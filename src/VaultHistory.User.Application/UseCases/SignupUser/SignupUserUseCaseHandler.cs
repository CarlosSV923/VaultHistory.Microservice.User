using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Commands.CreateUser;
using VaultHistory.User.Application.Providers.Jwt;
using VaultHistory.User.Application.Providers.PasswordHasher;
using VaultHistory.User.Application.Queries.VerifyUserEmailExists;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Domain.Users.ValueObjects;

namespace VaultHistory.User.Application.UseCases.SignupUser
{
    internal sealed class SignupUserUseCaseHandler(
        IMediator mediator,
        IPasswordHasherProvider passwordHasherProvider,
        IJwtProvider jwtProvider
    ) : IUseCaseHandler<SignupUserUseCase, SignupUserResponseDTO>
    {


        public async Task<Result<SignupUserResponseDTO>> Handle(SignupUserUseCase request, CancellationToken cancellationToken)
        {
            var body = request.Request;

            var emailResult = Email.Create(body.Email);

            if (emailResult.IsFailure)
            {
                return Result.Failure<SignupUserResponseDTO>(emailResult.Error);
            }

            // Check if user with the same email already exists
            var existingUserResult = await mediator.Send(new VerifyUserEmailExistsQuery(emailResult.Value), cancellationToken);
            if (existingUserResult.IsFailure)
            {
                return Result.Failure<SignupUserResponseDTO>(existingUserResult.Error);
            }

            // validate password
            var validatePasswordResult = passwordHasherProvider.ValidatePassword(body.Password);
            if (validatePasswordResult.IsFailure)
            {
                return Result.Failure<SignupUserResponseDTO>(validatePasswordResult.Error);
            }

            // Hash the password
            var passwordHashResult = passwordHasherProvider.HashPassword(body.Password);
            var passwordCreate = Password.Create(passwordHashResult.Hash, passwordHashResult.Salt);
            if (passwordCreate.IsFailure)
            {
                return Result.Failure<SignupUserResponseDTO>(UserErrors.PasswordHashingFailed);
            }

            var fullNameCreateResult = FullName.Create(body.FirstName, body.LastName);

            if (fullNameCreateResult.IsFailure)
            {
                return Result.Failure<SignupUserResponseDTO>(fullNameCreateResult.Error);
            }


            // Create new user
            var user = Domain.Users.User.Create(
                new CreateUserData(
                    fullNameCreateResult.Value,
                    emailResult.Value,
                    passwordCreate.Value,
                    body.DateOfBirth
                )
            );

            if (user.IsFailure)
            {
                return Result.Failure<SignupUserResponseDTO>(user.Error);
            }            

            // Save user to repository
            await mediator.Send(new CreateUserCommand(user.Value), cancellationToken);

            // Generate JWT token
            var token = jwtProvider.GenerateToken(user.Value);

            return Result.Success(new SignupUserResponseDTO(token.Token, token.Expiration));
        }
    }
}