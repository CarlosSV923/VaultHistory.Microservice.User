namespace VaultHistory.User.Domain.Users.ValueObjects
{
    public record UpdateUserData(
        FullName? FullName = null,
        bool UpdateBirthDate = false,
        DateOnly? BirthDate = null);
}