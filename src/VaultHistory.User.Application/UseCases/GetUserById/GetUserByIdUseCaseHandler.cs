using MediatR;
using Microsoft.Extensions.Logging;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;


namespace VaultHistory.User.Application.UseCases.GetUserById
{
    internal class GetUserByIdUseCaseHandler (
        IMediator mediator,
        ILogger<GetUserByIdUseCaseHandler> logger
    ) : IUseCaseHandler<GetUserByIdRequestDto, GetUserByIdResponseDto>
    {

        public async Task<Result<GetUserByIdResponseDto>> Handle(GetUserByIdRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;

            logger.LogInformation("Handling GetUserByIdUseCase for UserId: {UserId}", body.UserId);

            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                logger.LogWarning("Failed to parse UserId '{UserId}': {ErrorMessage}", body.UserId, userIdResult.Error.Message);
                return Result.Failure<GetUserByIdResponseDto>(userIdResult.Error);
            }

            var userResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (userResult.IsFailure)
            {
                logger.LogWarning("Failed to retrieve user with ID '{UserId}': {ErrorMessage}", body.UserId, userResult.Error.Message);
                return Result.Failure<GetUserByIdResponseDto>(userResult.Error);
            }

            var user = userResult.Value;
            var response = new GetUserByIdResponseDto(
                user.Id.ToString(),
                user.FullName.FirstName,
                user.FullName.LastName,
                user.Email.Value,
                user.BirthDate,
                user.IsActive,
                user.Notification,
                user.Theme,
                user.Character
            );

            logger.LogInformation("Returning user information for UserId '{UserId}': {UserId}", body.UserId, user.Id);
            return Result.Success(response);
        }
    }
}
