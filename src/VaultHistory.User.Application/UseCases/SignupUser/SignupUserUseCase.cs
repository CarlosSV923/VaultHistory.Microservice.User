using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.SignupUser
{
    public record SignupUserUseCase(
        SignupUserRequestDTO Request
    ) : IUseCase<SignupUserResponseDTO>;
}