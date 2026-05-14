using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;
using VaultHistory.User.Infrastructure.IntegrationTests.Shared;

namespace VaultHistory.User.Infrastructure.IntegrationTests.Repositories.UserRepository;

[Collection(PostgresCollection.Name)]
public sealed class GetByIdAsyncTests(PostgresFixture fixture)
{
    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
    {
        await fixture.ResetDatabaseAsync();

        var user = UserTestData.CreateUser("id-user@test.com");

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            repository.Add(user);
            await unitOfWork.SaveChangesAsync();
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var loaded = await repository.GetByIdAsync(user.Id);

            Assert.NotNull(loaded);
            Assert.Equal(user.Id, loaded!.Id);
            Assert.Equal("id-user@test.com", loaded.Email.Value);
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        await fixture.ResetDatabaseAsync();

        using var scope = fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var loaded = await repository.GetByIdAsync(Domain.Users.UserId.NewId());

        Assert.Null(loaded);
    }
}
