using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Commands.UpdateUser;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.ValueObjects;

namespace VaultHistory.User.Application.UseCases.UpdateUser
{
    internal sealed class UpdateUserUseCaseHandler(
        IMediator mediator
    ) : IUseCaseHandler<UpdateUserUseCase, UpdateUserResponseDTO>
    {
        public async Task<Result<UpdateUserResponseDTO>> Handle(UpdateUserUseCase request, CancellationToken cancellationToken)
        {
            var body = request.Request;

            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<UpdateUserResponseDTO>(userIdResult.Error);
            }

            var userResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (userResult.IsFailure)
            {
                return Result.Failure<UpdateUserResponseDTO>(userResult.Error);
            }

            var user = userResult.Value;

            var newFullNameResult = FullName.Create(body.FirstName, body.LastName);
            if (newFullNameResult.IsFailure)
            {
                return Result.Failure<UpdateUserResponseDTO>(newFullNameResult.Error);
            }

            var dataUpdate = new UpdateUserData(
                FullName: newFullNameResult.Value,
                UpdateBirthDate: body.DateOfBirth.HasValue,
                BirthDate: body.DateOfBirth
            );

            var updateResult = user.Update(dataUpdate);
            if (updateResult.IsFailure)
            {
                return Result.Failure<UpdateUserResponseDTO>(updateResult.Error);
            }

            var saveResult = await mediator.Send(new UpdateUserCommand(user), cancellationToken);
            if (saveResult.IsFailure)
            {
                return Result.Failure<UpdateUserResponseDTO>(saveResult.Error);
            }

            return Result.Success(new UpdateUserResponseDTO(user.Id.ToString()));
        }
    }
}