using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.DeactivateUser
{
    public record DeactivateUserRequestDto(
        string UserId
    ) : IUseCase<DeactivateUserResponseDto>;

    public record DeactivateUserResponseDto(
        string UserId
    );
}