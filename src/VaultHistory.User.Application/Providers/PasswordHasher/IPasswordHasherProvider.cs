using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Application.Providers.PasswordHasher
{
     public interface IPasswordHasherProvider
    {
        Result<PasswordHasherResult> HashPassword(string password);
        Result<bool> VerifyPassword(string password, string hash, string salt);
    }
}