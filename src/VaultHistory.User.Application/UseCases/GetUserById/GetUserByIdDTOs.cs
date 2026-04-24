namespace VaultHistory.User.Application.UseCases.GetUserById
{
    public record GetUserByIdRequestDTO(
        string UserId
    );

    public record GetUserByIdResponseDTO(
        string UserId,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate,
        bool IsActive
    );   
}