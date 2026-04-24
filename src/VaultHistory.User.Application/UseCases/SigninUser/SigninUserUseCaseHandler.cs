using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Providers.Jwt;
using VaultHistory.User.Application.Providers.PasswordHasher;
using VaultHistory.User.Application.Queries.GetUserByEmail;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.UseCases.SigninUser
{
    internal sealed class SigninUserUseCaseHandler(
        IMediator mediator,
        IPasswordHasherProvider passwordHasherProvider,
        IJwtProvider jwtProvider
    ) : IUseCaseHandler<SigninUserUseCase, SigninUserResponseDTO>
    {

        public async Task<Result<SigninUserResponseDTO>> Handle(SigninUserUseCase request, CancellationToken cancellationToken)
        {
            var body = request.Request;

            var emailResult = Email.Create(body.Email);
            if (emailResult.IsFailure)
            {
                return Result.Failure<SigninUserResponseDTO>(emailResult.Error);
            }

            var getUserResult = await mediator.Send(new GetUserByEmailQuery(emailResult.Value), cancellationToken);
            if (getUserResult.IsFailure)
            {
                return Result.Failure<SigninUserResponseDTO>(getUserResult.Error);
            }

            var user = getUserResult.Value;
            if (!user.IsActive)
            {
                return Result.Failure<SigninUserResponseDTO>(UserErrors.UserInactive);
            }

            var passwordVerificationResult = passwordHasherProvider.VerifyPassword(body.Password, user.Password.Hash, user.Password.Salt);
            if (passwordVerificationResult.IsFailure)
            {
                return Result.Failure<SigninUserResponseDTO>(UserErrors.InvalidPassword);
            }

            // Generate JWT token
            var token = jwtProvider.GenerateToken(user);

            return Result.Success(new SigninUserResponseDTO(token.Token, token.Expiration));
        }

       
        
    }
}