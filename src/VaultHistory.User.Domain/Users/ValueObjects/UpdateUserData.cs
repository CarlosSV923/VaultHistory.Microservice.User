namespace VaultHistory.User.Domain.Users.ValueObjects
{
    public record UpdateUserData(
        FullName? FullName = null,
        bool UpdateBirthDate = false,
        DateOnly? BirthDate = null,
        bool? Notification = null,
        string? Theme = null,
        string? Character = null);
}
