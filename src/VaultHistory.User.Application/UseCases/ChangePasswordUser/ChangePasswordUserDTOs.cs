namespace VaultHistory.User.Application.UseCases.ChangePasswordUser
{
    public record ChangePasswordUserRequestDTO(
        string UserId,
        string CurrentPassword,
        string NewPassword
    );

    public record ChangePasswordUserResponseDTO(
        string UserId
    );
}