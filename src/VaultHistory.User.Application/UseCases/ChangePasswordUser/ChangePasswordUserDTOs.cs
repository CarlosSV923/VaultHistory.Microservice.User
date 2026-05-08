using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.ChangePasswordUser
{
    public record ChangePasswordUserRequestDto(
        string UserId,
        string CurrentPassword,
        string NewPassword
    ) : IUseCase<ChangePasswordUserResponseDto>;

    public record ChangePasswordUserResponseDto(
        string UserId
    );
}