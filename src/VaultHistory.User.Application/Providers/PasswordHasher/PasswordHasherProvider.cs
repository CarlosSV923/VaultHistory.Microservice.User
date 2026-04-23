using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using VaultHistory.User.Application.Options;
using VaultHistory.User.Domain.Abstractions;

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

        private Result ValidatePassword(string password)
        {
            if (password.Length < _passwordOptions.MinLength)
                return Result.Failure(Error.Build(400, "PasswordTooShort", $"Password must be at least {_passwordOptions.MinLength} characters long."));

            if (!string.IsNullOrEmpty(_passwordOptions.Regex))
            {
                var regex = new System.Text.RegularExpressions.Regex(_passwordOptions.Regex);
                if (!regex.IsMatch(password))
                    return Result.Failure(Error.Build(400, "PasswordInvalidFormat", $"Password does not match the required format."));
            }

            return Result.Success();
        }

        public Result<PasswordHasherResult> HashPassword(string password)
        {
            var validationResult = ValidatePassword(password);
            if (!validationResult.IsSucceeded)
            {
                return Result.Failure<PasswordHasherResult>(validationResult.Error);
            }
            try
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

                return Result.Success(new PasswordHasherResult(hashBase64, saltBase64));

            }
            catch (Exception)
            {
                return Result.Failure<PasswordHasherResult>(Error.BuildInternalError("HashingFailed", $"Failed to hash password"));
            }
        }

        public Result<bool> VerifyPassword(string password, string hash, string salt)
        {
            var validationResult = ValidatePassword(password);
            if (!validationResult.IsSucceeded)
            {
                return Result.Failure<bool>(validationResult.Error);
            }
            try
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
                return Result.Success(CryptographicOperations.FixedTimeEquals(hashBytes, expectedBytes));
            }
            catch (Exception)
            {
                return Result.Failure<bool>(Error.BuildInternalError("HashingFailed", $"Failed to verify password"));
            }

        }
    }
}