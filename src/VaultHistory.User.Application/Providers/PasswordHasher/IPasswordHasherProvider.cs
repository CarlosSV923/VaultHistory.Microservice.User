using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Application.Providers.PasswordHasher
{
     public interface IPasswordHasherProvider
    {
        PasswordHasherResult HashPassword(string password);
        Result VerifyPassword(string password, string hash, string salt);
        Result ValidatePassword(string password);
    }
}