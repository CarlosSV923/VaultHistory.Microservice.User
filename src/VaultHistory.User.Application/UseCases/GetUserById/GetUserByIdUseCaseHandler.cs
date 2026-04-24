using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;


namespace VaultHistory.User.Application.UseCases.GetUserById
{
    internal class GetUserByIdUseCaseHandler (
        IMediator mediator
    ) : IUseCaseHandler<GetUserByIdUseCase, GetUserByIdResponseDTO>
    {

        public async Task<Result<GetUserByIdResponseDTO>> Handle(GetUserByIdUseCase request, CancellationToken cancellationToken)
        {
            var body = request.Request;
            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<GetUserByIdResponseDTO>(userIdResult.Error);
            }

            var userResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (userResult.IsFailure)
            {
                return Result.Failure<GetUserByIdResponseDTO>(userResult.Error);
            }

            var user = userResult.Value;
            var response = new GetUserByIdResponseDTO(
                user.Id.ToString(),
                user.FullName.FirstName,
                user.FullName.LastName,
                user.Email.Value,
                user.BirthDate,
                user.IsActive
            );

            return Result.Success(response);
        }
    }
}