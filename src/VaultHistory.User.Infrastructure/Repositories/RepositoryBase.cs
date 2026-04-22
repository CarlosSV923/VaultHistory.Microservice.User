using Microsoft.EntityFrameworkCore;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Infrastructure.Database;

namespace VaultHistory.User.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity, TId>(ApplicationDbContext context) where TEntity : Entity<TId> where TId : class
    {
        protected readonly ApplicationDbContext _context = context;

        public virtual void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public virtual Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            return _context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

    }
}