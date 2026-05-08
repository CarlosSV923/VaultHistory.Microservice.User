using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.SigninUser
{
    public record SigninUserRequestDto(
        string Email,
        string Password
    ) : IUseCase<SigninUserResponseDto>;

    public record SigninUserResponseDto(
        string Token,
        DateTime Expiration
    );    
}