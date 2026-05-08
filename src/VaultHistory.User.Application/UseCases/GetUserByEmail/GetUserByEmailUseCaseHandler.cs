using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Queries.GetUserByEmail;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.UseCases.GetUserByEmail
{
    internal sealed class GetUserByEmailUseCaseHandler(
        IMediator mediator
    ) : IUseCaseHandler<GetUserByEmailRequestDto, GetUserByEmailResponseDto>
    {
        public async Task<Result<GetUserByEmailResponseDto>> Handle(GetUserByEmailRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;

            var emaulResult = Email.Create(body.Email);

            if (emaulResult.IsFailure)
            {
                return Result.Failure<GetUserByEmailResponseDto>(emaulResult.Error);
            }

            var userResult = await mediator.Send(new GetUserByEmailQuery(emaulResult.Value), cancellationToken);
            if (userResult.IsFailure)
            {
                return Result.Failure<GetUserByEmailResponseDto>(userResult.Error);
            }

            var user = userResult.Value;

            var response = new GetUserByEmailResponseDto(
                user.Id.ToString(),
                user.FullName.FirstName,
                user.FullName.LastName,
                user.Email.ToString(),
                user.BirthDate,
                user.IsActive
            );

            return Result.Success(response);
        }
    }
}