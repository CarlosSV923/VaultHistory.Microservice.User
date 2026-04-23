using System.Reflection.Metadata;

namespace VaultHistory.User.Domain.Users.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
        void Update(User user);
        Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);

        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

        Task<bool> VerifyEmailExistsAsync(Email email, CancellationToken cancellationToken = default);

    }
}