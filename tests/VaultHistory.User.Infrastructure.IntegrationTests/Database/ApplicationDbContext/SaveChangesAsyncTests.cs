using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;
using VaultHistory.User.Infrastructure.IntegrationTests.Shared;
using VaultHistory.User.Infrastructure.Outbox;

namespace VaultHistory.User.Infrastructure.IntegrationTests.Database.ApplicationDbContext;

[Collection(PostgresCollection.Name)]
public sealed class SaveChangesAsyncTests(PostgresFixture fixture)
{
    [Fact]
    public async Task SaveChangesAsync_WhenUserIsAdded_PersistsEntity()
    {
        await fixture.ResetDatabaseAsync();

        using var scope = fixture.ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<User.Infrastructure.Database.ApplicationDbContext>();

        var user = UserTestData.CreateUser("outbox@test.com");
        dbContext.Set<Domain.Users.User>().Add(user);

        await dbContext.SaveChangesAsync();

        var persistedUser = await dbContext.Set<Domain.Users.User>()
            .FirstOrDefaultAsync(u => u.Email.Value == "outbox@test.com");

        Assert.NotNull(persistedUser);
        Assert.Equal(user.Id, persistedUser!.Id);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenOutboxMessageIsAdded_PersistsOutboxDefaults()
    {
        await fixture.ResetDatabaseAsync();

        using var scope = fixture.ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<User.Infrastructure.Database.ApplicationDbContext>();

        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "TestEvent",
            Payload = "{\"test\":true}",
            OccurredOn = DateTime.UtcNow
        };

        dbContext.Set<OutboxMessage>().Add(message);
        await dbContext.SaveChangesAsync();

        var persisted = await dbContext.Set<OutboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == message.Id);

        Assert.NotNull(persisted);
        Assert.Equal("PENDING", persisted!.Status);
        Assert.Null(persisted.UpdateAt);
        Assert.Null(persisted.Error);
    }
}
