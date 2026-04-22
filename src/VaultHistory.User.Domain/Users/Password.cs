using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users
{
    public sealed record Password
    {
        private Password(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }

        public string Hash { get; }
        public string Salt { get; }

        public static Result<Password> Create(string hash, string salt)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return Result.Failure<Password>(UserErrors.PasswordHashRequired);
            }

            if (string.IsNullOrWhiteSpace(salt))
            {
                return Result.Failure<Password>(UserErrors.PasswordSaltRequired);
            }

            return Result.Success(new Password(hash.Trim(), salt.Trim()));
        }
    }
}