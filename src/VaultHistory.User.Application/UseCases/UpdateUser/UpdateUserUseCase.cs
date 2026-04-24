using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.UpdateUser
{
    public record UpdateUserUseCase(
        UpdateUserRequestDTO Request
    ) : IUseCase<UpdateUserResponseDTO>;
}