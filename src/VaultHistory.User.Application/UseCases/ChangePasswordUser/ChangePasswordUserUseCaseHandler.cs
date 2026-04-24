using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Providers.PasswordHasher;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.UseCases.ChangePasswordUser
{
    internal sealed class ChangePasswordUserUseCaseHandler(
        IMediator mediator,
        IPasswordHasherProvider passwordHasherProvider
    ) : IUseCaseHandler<ChangePasswordUserUseCase, ChangePasswordUserResponseDTO>
    {
        private readonly IPasswordHasherProvider _passwordHasherProvider = passwordHasherProvider;
        public async Task<Result<ChangePasswordUserResponseDTO>> Handle(ChangePasswordUserUseCase request, CancellationToken cancellationToken)
        {
            var body = request.Request;

            var userIdResult = UserId.FromString(body.UserId);
            if (userIdResult.IsFailure)
            {
                return Result.Failure<ChangePasswordUserResponseDTO>(userIdResult.Error);
            }

            var userResult = await mediator.Send(new GetUserByIdQuery(userIdResult.Value), cancellationToken);
            if (userResult.IsFailure)
            {
                return Result.Failure<ChangePasswordUserResponseDTO>(userResult.Error);
            }
            var user = userResult.Value;

            var verifyPasswordResult = _passwordHasherProvider.VerifyPassword(body.CurrentPassword, user.Password.Hash, user.Password.Salt);
            if (verifyPasswordResult.IsFailure)
            {
                return Result.Failure<ChangePasswordUserResponseDTO>(verifyPasswordResult.Error);
            }

            var validatePasswordResult = _passwordHasherProvider.ValidatePassword(body.NewPassword);
            if (validatePasswordResult.IsFailure)
            {
                return Result.Failure<ChangePasswordUserResponseDTO>(validatePasswordResult.Error);
            }

            var hashData = _passwordHasherProvider.HashPassword(body.NewPassword);

            var passwordReult = Password.Create(hashData.Hash, hashData.Salt);
            if (passwordReult.IsFailure)
            {
                return Result.Failure<ChangePasswordUserResponseDTO>(passwordReult.Error);
            }

            user.ChangePassword(passwordReult.Value);

            var updateResult = await mediator.Send(new Commands.UpdateUser.UpdateUserCommand(user), cancellationToken);
            if (updateResult.IsFailure)
            {
                return Result.Failure<ChangePasswordUserResponseDTO>(updateResult.Error);
            }

            return Result.Success(new ChangePasswordUserResponseDTO(user.Id.ToString()));
        }
    }
}