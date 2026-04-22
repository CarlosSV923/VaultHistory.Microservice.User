namespace VaultHistory.User.Domain.Users
{
    public record UserId(Guid Value)
    {
        public static UserId NewId() => new(Guid.NewGuid());
    }
}