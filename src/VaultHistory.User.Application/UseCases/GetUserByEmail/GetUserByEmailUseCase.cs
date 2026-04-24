using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.GetUserByEmail
{
    public record GetUserByEmailUseCase(
        GetUserByEmailRequestDTO Request
    ) : IUseCase<GetUserByEmailResponseDTO>;
}