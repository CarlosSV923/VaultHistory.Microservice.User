namespace VaultHistory.User.Application.UseCases.GetUserByEmail
{
    public record GetUserByEmailRequestDTO(
        string Email
    );

    public record GetUserByEmailResponseDTO(
        string UserId,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate,
        bool IsActive
    );
}