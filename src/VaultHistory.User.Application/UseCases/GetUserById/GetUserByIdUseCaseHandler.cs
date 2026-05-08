using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;


namespace VaultHistory.User.Application.UseCases.GetUserById
{
    internal class GetUserByIdUseCaseHandler (
        IMediator mediator
    ) : IUseCaseHandler<GetUserByIdRequestDto, GetUserByIdResponseDto>
    {

        public async Task<Result<GetUserByIdResponseDto>> Handle(GetUserByIdRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;
            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<GetUserByIdResponseDto>(userIdResult.Error);
            }

            var userResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (userResult.IsFailure)
            {
                return Result.Failure<GetUserByIdResponseDto>(userResult.Error);
            }

            var user = userResult.Value;
            var response = new GetUserByIdResponseDto(
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