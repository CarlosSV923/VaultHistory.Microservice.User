namespace VaultHistory.User.Application.UseCases.SignupUser
{
    public record SignupUserRequestDTO(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        DateOnly? DateOfBirth
    );

    public record SignupUserResponseDTO(
        string Token,
        DateTime Expiration
    );
}