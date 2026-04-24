namespace VaultHistory.User.Application.UseCases.SigninUser
{
    public record SigninUserRequestDTO(
        string Email,
        string Password
    );

    public record SigninUserResponseDTO(
        string Token,
        DateTime Expiration
    );    
}