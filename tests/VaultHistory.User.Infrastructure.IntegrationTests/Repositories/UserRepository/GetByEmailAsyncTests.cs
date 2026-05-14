using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;
using VaultHistory.User.Infrastructure.IntegrationTests.Shared;

namespace VaultHistory.User.Infrastructure.IntegrationTests.Repositories.UserRepository;

[Collection(PostgresCollection.Name)]
public sealed class GetByEmailAsyncTests(PostgresFixture fixture)
{
    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
    {
        await fixture.ResetDatabaseAsync();

        var user = UserTestData.CreateUser("ana@test.com");

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
            var emailResult = Email.Create("ana@test.com");
            Assert.True(emailResult.IsSucceeded);

            var found = await repository.GetByEmailAsync(emailResult.Value);

            Assert.NotNull(found);
            Assert.Equal(user.Id, found!.Id);
            Assert.Equal("ana@test.com", found.Email.Value);
        }
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        await fixture.ResetDatabaseAsync();

        using var scope = fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var emailResult = Email.Create("missing@test.com");
        Assert.True(emailResult.IsSucceeded);

        var found = await repository.GetByEmailAsync(emailResult.Value);

        Assert.Null(found);
    }
}
