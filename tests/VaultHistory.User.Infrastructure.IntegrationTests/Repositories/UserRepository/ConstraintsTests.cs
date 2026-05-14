using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;
using VaultHistory.User.Infrastructure.IntegrationTests.Shared;

namespace VaultHistory.User.Infrastructure.IntegrationTests.Repositories.UserRepository;

[Collection(PostgresCollection.Name)]
public sealed class ConstraintsTests(PostgresFixture fixture)
{
    [Fact]
    public async Task Add_WhenDuplicateEmailExists_ThrowsDbUpdateException()
    {
        await fixture.ResetDatabaseAsync();

        using var scope = fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        repository.Add(UserTestData.CreateUser("duplicate@test.com"));
        await unitOfWork.SaveChangesAsync();

        repository.Add(UserTestData.CreateUser("duplicate@test.com"));

        await Assert.ThrowsAsync<DbUpdateException>(() => unitOfWork.SaveChangesAsync());
    }
}
