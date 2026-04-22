namespace VaultHistory.User.Domain.Users.ValueObjects
{
    public record UpdateUserData(
        FullName? FullName = null,
        Email? Email = null,
        Password? Password = null,
        bool UpdateBirthDate = false,
        DateOnly? BirthDate = null);
}