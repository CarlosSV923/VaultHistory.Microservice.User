using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.SigninUser
{
    public record SigninUserUseCase(
        SigninUserRequestDTO Request
    ) : IUseCase<SigninUserResponseDTO>;
}