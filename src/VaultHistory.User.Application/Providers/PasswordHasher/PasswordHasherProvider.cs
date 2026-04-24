using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using VaultHistory.User.Application.Options;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.Providers.PasswordHasher
{
    public sealed class PasswordHasherProvider(
        IOptions<HashingOptions> hashingOptions,
        IOptions<PasswordOptions> passwordOptions
    ) : IPasswordHasherProvider
    {
        private readonly HashingOptions _hashingOptions = hashingOptions.Value;
        private readonly PasswordOptions _passwordOptions = passwordOptions.Value;

        private static byte[] GenerateSalt(int length = 16)
        {
            var salt = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public Result ValidatePassword(string password)
        {
            if (password.Length < _passwordOptions.MinLength)
                return Result.Failure(UserErrors.PasswordTooShort);

            if (!string.IsNullOrEmpty(_passwordOptions.Regex))
            {
                var regex = new System.Text.RegularExpressions.Regex(_passwordOptions.Regex);
                if (!regex.IsMatch(password))
                    return Result.Failure(UserErrors.PasswordInvalidFormat);
            }

            return Result.Success();
        }

        public PasswordHasherResult HashPassword(string password)
        {

            var salt = GenerateSalt(_hashingOptions.SaltSize);
            var saltBase64 = Convert.ToBase64String(salt);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                MemorySize = _hashingOptions.MemorySize, // in KB
                DegreeOfParallelism = _hashingOptions.DegreeOfParallelism,
                Iterations = _hashingOptions.Iterations
            };

            var hash = argon2.GetBytes(_hashingOptions.HashSize);
            var hashBase64 = Convert.ToBase64String(hash);

            return new PasswordHasherResult(hashBase64, saltBase64);

        }

        public Result VerifyPassword(string password, string hash, string salt)
        {

            var saltBytes = Convert.FromBase64String(salt);
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = saltBytes,
                MemorySize = _hashingOptions.MemorySize, // in KB
                DegreeOfParallelism = _hashingOptions.DegreeOfParallelism,
                Iterations = _hashingOptions.Iterations
            };

            var hashBytes = argon2.GetBytes(_hashingOptions.HashSize);
            var expectedBytes = Convert.FromBase64String(hash);
            var isValid = CryptographicOperations.FixedTimeEquals(hashBytes, expectedBytes);
            if (!isValid)
            {
                return Result.Failure(UserErrors.InvalidPassword);
            }
            return Result.Success();


        }
    }
}