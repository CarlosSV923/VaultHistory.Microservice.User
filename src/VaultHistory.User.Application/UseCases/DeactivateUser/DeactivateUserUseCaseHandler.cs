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
    ) : IUseCaseHandler<DeactivateUserUseCase, DeactivateUserResponseDTO>
    {


        public async Task<Result<DeactivateUserResponseDTO>> Handle(DeactivateUserUseCase request, CancellationToken cancellationToken)
        {
            var body = request.Request;
            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<DeactivateUserResponseDTO>(userIdResult.Error);
            }

            var getUserResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (getUserResult.IsFailure)
            {
                return Result.Failure<DeactivateUserResponseDTO>(getUserResult.Error);
            }

            var user = getUserResult.Value;
            user.Deactivate();

            var updateResult = await mediator.Send(new UpdateUserCommand(user), cancellationToken);
            if (updateResult.IsFailure)
            {
                return Result.Failure<DeactivateUserResponseDTO>(updateResult.Error);
            }

            return Result.Success(new DeactivateUserResponseDTO(user.Id.ToString()));
        }
    }
}