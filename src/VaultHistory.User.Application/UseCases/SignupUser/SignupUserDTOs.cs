using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.SignupUser
{
    public record SignupUserRequestDto(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        DateOnly? DateOfBirth,
        bool Notification = false,
        string? Theme = null,
        string? Character = null
    ) : IUseCase<SignupUserResponseDto>;

    public record SignupUserResponseDto(
        string Token,
        DateTime Expiration
    );
}
