using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.DeactivateUser
{
    public record DeactivateUserUseCase(
        DeactivateUserRequestDTO Request
    ) : IUseCase<DeactivateUserResponseDTO>;
}