namespace VaultHistory.User.Api.Controllers.V1.User
{
    public sealed record GetByIdRequest(string Id);

    public sealed record GetByEmailRequest(string Email);

    public sealed record SigninRequest(string Email, string Password);

    public sealed record SignupRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        DateOnly? BirthDate,
        bool Notification = false,
        string? Theme = null,
        string? Character = null);

    public sealed record UpdateRequest(
        string? FirstName,
        string? LastName,
        DateOnly? BirthDate,
        bool? Notification = null,
        string? Theme = null,
        string? Character = null);

    public sealed record DeactivateRequest(string Id);

    public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
