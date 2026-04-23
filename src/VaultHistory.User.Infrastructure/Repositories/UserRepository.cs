using Microsoft.EntityFrameworkCore;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Infrastructure.Database;

namespace VaultHistory.User.Infrastructure.Repositories
{
    internal sealed class UserRepository(ApplicationDbContext context) : RepositoryBase<Domain.Users.User, UserId>(context), IUserRepository
    {
        public Task<Domain.Users.User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return _context.Set<Domain.Users.User>().FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
        }

        public Task<bool> VerifyEmailExistsAsync(Email email, CancellationToken cancellationToken = default)
        {
            return _context.Set<Domain.Users.User>().AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
        }
    }
}