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
    ) : IUseCaseHandler<SigninUserRequestDto, SigninUserResponseDto>
    {

        public async Task<Result<SigninUserResponseDto>> Handle(SigninUserRequestDto request, CancellationToken cancellationToken)
        {
            var body = request;

            var emailResult = Email.Create(body.Email);
            if (emailResult.IsFailure)
            {
                return Result.Failure<SigninUserResponseDto>(emailResult.Error);
            }

            var getUserResult = await mediator.Send(new GetUserByEmailQuery(emailResult.Value), cancellationToken);
            if (getUserResult.IsFailure)
            {
                return Result.Failure<SigninUserResponseDto>(getUserResult.Error);
            }

            var user = getUserResult.Value;
            if (!user.IsActive)
            {
                return Result.Failure<SigninUserResponseDto>(UserErrors.UserInactive);
            }

            var passwordVerificationResult = passwordHasherProvider.VerifyPassword(body.Password, user.Password.Hash, user.Password.Salt);
            if (passwordVerificationResult.IsFailure)
            {
                return Result.Failure<SigninUserResponseDto>(UserErrors.InvalidPassword);
            }

            // Generate JWT token
            var token = jwtProvider.GenerateToken(user);

            return Result.Success(new SigninUserResponseDto(token.Token, token.Expiration));
        }

       
        
    }
}