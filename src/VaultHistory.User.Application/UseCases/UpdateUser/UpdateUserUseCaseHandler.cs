using MediatR;
using Microsoft.Extensions.Logging;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Commands.UpdateUser;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.ValueObjects;

namespace VaultHistory.User.Application.UseCases.UpdateUser
{
    internal sealed class UpdateUserUseCaseHandler(
        IMediator mediator,
        ILogger<UpdateUserUseCaseHandler> logger
    ) : IUseCaseHandler<UpdateUserRequestDto, UpdateUserResponseDto>
    {
        public async Task<Result<UpdateUserResponseDto>> Handle(UpdateUserRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;

            logger.LogInformation("Handling UpdateUserUseCase for UserId: {UserId}", body.UserId);

            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                logger.LogWarning("Failed to parse UserId '{UserId}': {ErrorMessage}", body.UserId, userIdResult.Error.Message);
                return Result.Failure<UpdateUserResponseDto>(userIdResult.Error);
            }

            var userResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (userResult.IsFailure)
            {
                logger.LogWarning("Failed to retrieve user with ID '{UserId}': {ErrorMessage}", body.UserId, userResult.Error.Message);
                return Result.Failure<UpdateUserResponseDto>(userResult.Error);
            }

            var user = userResult.Value;

            var shouldUpdateFullName = body.FirstName is not null || body.LastName is not null;
            var newFullNameResult = shouldUpdateFullName
                ? FullName.Create(body.FirstName ?? user.FullName.FirstName, body.LastName ?? user.FullName.LastName)
                : Result.Success(user.FullName);
            if (newFullNameResult.IsFailure)
            {
                logger.LogWarning("Failed to create FullName for UserId '{UserId}': {ErrorMessage}", body.UserId, newFullNameResult.Error.Message);
                return Result.Failure<UpdateUserResponseDto>(newFullNameResult.Error);
            }

            var dataUpdate = new UpdateUserData(
                FullName: shouldUpdateFullName ? newFullNameResult.Value : null,
                UpdateBirthDate: body.DateOfBirth.HasValue,
                BirthDate: body.DateOfBirth,
                Notification: body.Notification,
                Theme: body.Theme,
                Character: body.Character
            );

            var updateResult = user.Update(dataUpdate);
            if (updateResult.IsFailure)
            {
                logger.LogWarning("Failed to update user with ID '{UserId}': {ErrorMessage}", body.UserId, updateResult.Error.Message);
                return Result.Failure<UpdateUserResponseDto>(updateResult.Error);
            }

            var saveResult = await mediator.Send(new UpdateUserCommand(user), cancellationToken);
            if (saveResult.IsFailure)
            {
                logger.LogWarning("Failed to save updated user with ID '{UserId}': {ErrorMessage}", body.UserId, saveResult.Error.Message);
                return Result.Failure<UpdateUserResponseDto>(saveResult.Error);
            }

            logger.LogInformation("User with ID '{UserId}' updated successfully", body.UserId);
            return Result.Success(new UpdateUserResponseDto(user.Id.ToString()));
        }
    }
}
