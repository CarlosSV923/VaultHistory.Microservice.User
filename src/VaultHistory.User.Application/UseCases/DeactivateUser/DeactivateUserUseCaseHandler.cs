using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Commands.UpdateUser;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.UseCases.DeactivateUser
{
    internal sealed class DeactivateUserUseCaseHandler(
        IMediator mediator
    ) : IUseCaseHandler<DeactivateUserRequestDto, DeactivateUserResponseDto>
    {


        public async Task<Result<DeactivateUserResponseDto>> Handle(DeactivateUserRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;
            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<DeactivateUserResponseDto>(userIdResult.Error);
            }

            var getUserResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (getUserResult.IsFailure)
            {
                return Result.Failure<DeactivateUserResponseDto>(getUserResult.Error);
            }

            var user = getUserResult.Value;
            user.Deactivate();

            var updateResult = await mediator.Send(new UpdateUserCommand(user), cancellationToken);
            if (updateResult.IsFailure)
            {
                return Result.Failure<DeactivateUserResponseDto>(updateResult.Error);
            }

            return Result.Success(new DeactivateUserResponseDto(user.Id.ToString()));
        }
    }
}