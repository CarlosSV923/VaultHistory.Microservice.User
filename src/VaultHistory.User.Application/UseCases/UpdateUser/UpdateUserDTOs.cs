namespace VaultHistory.User.Application.UseCases.UpdateUser
{
    public record UpdateUserRequestDTO(
        string UserId,
        string? FirstName,
        string? LastName,
        DateOnly? DateOfBirth
    );

    public record UpdateUserResponseDTO(
        string UserId
    );
}