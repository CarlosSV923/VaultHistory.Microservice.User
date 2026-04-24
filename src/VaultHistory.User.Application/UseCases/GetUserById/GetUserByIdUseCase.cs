using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.GetUserById
{
    public record GetUserByIdUseCase(
        GetUserByIdRequestDTO Request
    ) : IUseCase<GetUserByIdResponseDTO>;
}