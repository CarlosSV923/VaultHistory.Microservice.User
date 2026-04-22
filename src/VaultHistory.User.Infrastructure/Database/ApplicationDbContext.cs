using Microsoft.EntityFrameworkCore;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Infrastructure.Database
{
    public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
           base.OnModelCreating(modelBuilder);
        }
    }
}