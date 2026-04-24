using Microsoft.EntityFrameworkCore;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Infrastructure.Outbox;
using Newtonsoft.Json;
using VaultHistory.User.Application.Exceptions;

namespace VaultHistory.User.Infrastructure.Database
{
    public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        private void AddDomainEventsToOutbox()
        {
            var outboxMessages = ChangeTracker.Entries<IEntity>()
                .Select(e => e.Entity)
                .SelectMany(e =>
                {
                    var events = e.GetDomainEvents();
                    e.ClearDomainEvents();
                    return events;
                })
                .Select(e => new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = e.GetType().Name,
                    Payload = JsonConvert.SerializeObject(e, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    }),
                    OccurredOn = DateTime.UtcNow
                })
                .ToList();

            Set<OutboxMessage>().AddRange(outboxMessages);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                AddDomainEventsToOutbox();
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConsurrencyException("A concurrency error occurred while saving changes.", ex);
            }
        }
    }
}