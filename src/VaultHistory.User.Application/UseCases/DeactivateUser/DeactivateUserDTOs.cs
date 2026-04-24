namespace VaultHistory.User.Application.UseCases.DeactivateUser
{
    public record DeactivateUserRequestDTO(
        string UserId
    );

    public record DeactivateUserResponseDTO(
        string UserId
    );
}