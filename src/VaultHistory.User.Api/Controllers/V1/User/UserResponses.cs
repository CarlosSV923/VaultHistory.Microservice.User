namespace VaultHistory.User.Api.Controllers.V1.User
{
    public sealed record GetByEmailResponse(
        string Id,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate,
        bool IsActive,
        bool Notification,
        string? Theme,
        string? Character);

    public sealed record GetByIdResponse(
        string Id,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate,
        bool IsActive,
        bool Notification,
        string? Theme,
        string? Character);
    public sealed record SigninResponse(string Token, DateTime Expiration);
    public sealed record SignupResponse(string Token, DateTime Expiration);
    public sealed record UpdateResponse(string Id);
    public sealed record DeactivateResponse(string Id);
    public sealed record ChangePasswordResponse(string Id);

}
