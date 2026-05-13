using MediatR;
using Microsoft.Extensions.Logging;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Providers.PasswordHasher;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.UseCases.ChangePasswordUser
{
    internal sealed class ChangePasswordUserUseCaseHandler(
        IMediator mediator,
        IPasswordHasherProvider passwordHasherProvider,
        ILogger<ChangePasswordUserUseCaseHandler> logger
    ) : IUseCaseHandler<ChangePasswordUserRequestDto, ChangePasswordUserResponseDto>
    {
        private readonly IPasswordHasherProvider _passwordHasherProvider = passwordHasherProvider;
        public async Task<Result<ChangePasswordUserResponseDto>> Handle(ChangePasswordUserRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;

            logger.LogInformation("Starting password change operation for UserId: {UserId}", body.UserId);

            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                logger.LogWarning("Failed to parse UserId '{UserId}': {ErrorMessage}", body.UserId, userIdResult.Error.Message);
                return Result.Failure<ChangePasswordUserResponseDto>(userIdResult.Error);
            }

            var userResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (userResult.IsFailure)
            {
                logger.LogWarning("Failed to retrieve user with UserId '{UserId}': {ErrorMessage}", body.UserId, userResult.Error.Message);
                return Result.Failure<ChangePasswordUserResponseDto>(userResult.Error);
            }
            var user = userResult.Value;

            var verifyPasswordResult = _passwordHasherProvider.VerifyPassword(body.CurrentPassword, user.Password.Hash, user.Password.Salt);
            if (verifyPasswordResult.IsFailure)
            {
                logger.LogWarning("Password verification failed for UserId '{UserId}': {ErrorMessage}", body.UserId, verifyPasswordResult.Error.Message);
                return Result.Failure<ChangePasswordUserResponseDto>(verifyPasswordResult.Error);
            }

            var validatePasswordResult = _passwordHasherProvider.ValidatePassword(body.NewPassword);
            if (validatePasswordResult.IsFailure)
            {
                logger.LogWarning("New password validation failed for UserId '{UserId}': {ErrorMessage}", body.UserId, validatePasswordResult.Error.Message);
                return Result.Failure<ChangePasswordUserResponseDto>(validatePasswordResult.Error);
            }

            var hashData = _passwordHasherProvider.HashPassword(body.NewPassword);

            var passwordResult = Password.Create(hashData.Hash, hashData.Salt);
            if (passwordResult.IsFailure)
            {
                logger.LogWarning("Failed to create password object for UserId '{UserId}': {ErrorMessage}", body.UserId, passwordResult.Error.Message);
                return Result.Failure<ChangePasswordUserResponseDto>(passwordResult.Error);
            }

            user.ChangePassword(passwordResult.Value);

            var updateResult = await mediator.Send(new Commands.UpdateUser.UpdateUserCommand(user), cancellationToken);
            if (updateResult.IsFailure)
            {
                logger.LogWarning("Failed to update user password for UserId '{UserId}': {ErrorMessage}", body.UserId, updateResult.Error.Message);
                return Result.Failure<ChangePasswordUserResponseDto>(updateResult.Error);
            }

            logger.LogInformation("UserId: {UserId} - Password changed successfully", body.UserId);
            return Result.Success(new ChangePasswordUserResponseDto(user.Id.ToString()));
        }
    }
}