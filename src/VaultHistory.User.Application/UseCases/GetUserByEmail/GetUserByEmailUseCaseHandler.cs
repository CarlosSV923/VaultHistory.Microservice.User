using MediatR;
using Microsoft.Extensions.Logging;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Queries.GetUserByEmail;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.UseCases.GetUserByEmail
{
    internal sealed class GetUserByEmailUseCaseHandler(
        IMediator mediator,
        ILogger<GetUserByEmailUseCaseHandler> logger
    ) : IUseCaseHandler<GetUserByEmailRequestDto, GetUserByEmailResponseDto>
    {
        public async Task<Result<GetUserByEmailResponseDto>> Handle(GetUserByEmailRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;

            logger.LogInformation("Handling GetUserByEmailUseCase for Email: {Email}", body.Email);

            var emailResult = Email.Create(body.Email);

            if (emailResult.IsFailure)
            {
                logger.LogWarning("Failed to parse Email '{Email}': {ErrorMessage}", body.Email, emailResult.Error.Message);
                return Result.Failure<GetUserByEmailResponseDto>(emailResult.Error);
            }

            var userResult = await mediator.Send(new GetUserByEmailQuery(emailResult.Value), cancellationToken);
            if (userResult.IsFailure)
            {
                logger.LogWarning("Failed to retrieve user with Email '{Email}': {ErrorMessage}", body.Email, userResult.Error.Message);
                return Result.Failure<GetUserByEmailResponseDto>(userResult.Error);
            }

            var user = userResult.Value;

            if (user.Id.ToString() != body.RequestingUserId)
            {
                logger.LogWarning("User with id '{RequestingUserId}' attempted to access information for user with ID '{UserId}'", body.RequestingUserId, user.Id);
                return Result.Failure<GetUserByEmailResponseDto>(UserErrors.InvalidUserId);
            }

            var response = new GetUserByEmailResponseDto(
                user.Id.ToString(),
                user.FullName.FirstName,
                user.FullName.LastName,
                user.Email.ToString(),
                user.BirthDate,
                user.IsActive
            );
            logger.LogInformation("Returning user information for Email '{Email}': {UserId}", body.Email, user.Id);
            return Result.Success(response);
        }
    }
}