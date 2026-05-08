using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users
{
    public record UserId(Guid Value)
    {
        public static UserId NewId() => new(Guid.NewGuid());
        public static Result<UserId> FromString(string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return Result.Failure<UserId>(UserErrors.InvalidUserId);
            }
            if (Guid.TryParse(str, out var guid))
            {
                return Result.Success(new UserId(guid));
            }
            return Result.Failure<UserId>(UserErrors.InvalidUserId);
        }

        public override string ToString() => Value.ToString();
    }
}