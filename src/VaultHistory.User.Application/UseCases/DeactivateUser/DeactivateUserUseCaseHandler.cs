using MediatR;
using Microsoft.Extensions.Logging;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Commands.UpdateUser;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.UseCases.DeactivateUser
{
    internal sealed class DeactivateUserUseCaseHandler(
        IMediator mediator,
        ILogger<DeactivateUserUseCaseHandler> logger
    ) : IUseCaseHandler<DeactivateUserRequestDto, DeactivateUserResponseDto>
    {


        public async Task<Result<DeactivateUserResponseDto>> Handle(DeactivateUserRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;

            logger.LogInformation("Handling DeactivateUserUseCase for UserId: {UserId}", body.UserId);

            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                logger.LogWarning("Failed to parse UserId '{UserId}': {ErrorMessage}", body.UserId, userIdResult.Error.Message);
                return Result.Failure<DeactivateUserResponseDto>(userIdResult.Error);
            }

            var getUserResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (getUserResult.IsFailure)
            {
                logger.LogWarning("Failed to retrieve user with ID '{UserId}': {ErrorMessage}", body.UserId, getUserResult.Error.Message);
                return Result.Failure<DeactivateUserResponseDto>(getUserResult.Error);
            }

            var user = getUserResult.Value;
            user.Deactivate();

            var updateResult = await mediator.Send(new UpdateUserCommand(user), cancellationToken);
            if (updateResult.IsFailure)
            {
                logger.LogWarning("Failed to update user with ID '{UserId}': {ErrorMessage}", body.UserId, updateResult.Error.Message);
                return Result.Failure<DeactivateUserResponseDto>(updateResult.Error);
            }

            logger.LogInformation("User with ID '{UserId}' deactivated successfully", body.UserId);
            return Result.Success(new DeactivateUserResponseDto(user.Id.ToString()));
        }
    }
}