namespace VaultHistory.User.Domain.Users.ValueObjects
{
    public record CreateUserData(
        FullName FullName,
        Email Email,
        Password Password,
        DateOnly? BirthDate = null,
        bool Notification = false,
        string? Theme = null,
        string? Character = null)
    {

    }
}
