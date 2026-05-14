using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;
using VaultHistory.User.Infrastructure.IntegrationTests.Shared;

namespace VaultHistory.User.Infrastructure.IntegrationTests.Repositories.UserRepository;

[Collection(PostgresCollection.Name)]
public sealed class VerifyEmailExistsAsyncTests(PostgresFixture fixture)
{
    [Fact]
    public async Task VerifyEmailExistsAsync_WhenUserExists_ReturnsTrue()
    {
        await fixture.ResetDatabaseAsync();

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            repository.Add(UserTestData.CreateUser("ana@test.com"));

            await unitOfWork.SaveChangesAsync();
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var emailResult = Email.Create("ana@test.com");
            Assert.True(emailResult.IsSucceeded);

            var exists = await repository.VerifyEmailExistsAsync(emailResult.Value);

            Assert.True(exists);
        }
    }

    [Fact]
    public async Task VerifyEmailExistsAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        await fixture.ResetDatabaseAsync();

        using var scope = fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var emailResult = Email.Create("missing@test.com");
        Assert.True(emailResult.IsSucceeded);

        var exists = await repository.VerifyEmailExistsAsync(emailResult.Value);

        Assert.False(exists);
    }
}
