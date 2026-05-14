using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;
using VaultHistory.User.Infrastructure.IntegrationTests.Shared;

namespace VaultHistory.User.Infrastructure.IntegrationTests.Repositories.UserRepository;

[Collection(PostgresCollection.Name)]
public sealed class UpdateTests(PostgresFixture fixture)
{
    [Fact]
    public async Task Update_WhenEmailChanges_PersistsUpdatedValue()
    {
        await fixture.ResetDatabaseAsync();

        var user = UserTestData.CreateUser("before-update@test.com");

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            repository.Add(user);
            await unitOfWork.SaveChangesAsync();

            var newEmailResult = Email.Create("after-update@test.com");
            Assert.True(newEmailResult.IsSucceeded);
            user.ChangeEmail(newEmailResult.Value);

            repository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        using (var scope = fixture.ServiceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var updatedEmailResult = Email.Create("after-update@test.com");
            Assert.True(updatedEmailResult.IsSucceeded);

            var loaded = await repository.GetByEmailAsync(updatedEmailResult.Value);

            Assert.NotNull(loaded);
            Assert.Equal(user.Id, loaded!.Id);
            Assert.Equal("after-update@test.com", loaded.Email.Value);
        }
    }
}
