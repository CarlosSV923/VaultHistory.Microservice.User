using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.ChangePasswordUser
{
    public record ChangePasswordUserUseCase(
        ChangePasswordUserRequestDTO Request
    ) : IUseCase<ChangePasswordUserResponseDTO>;
}