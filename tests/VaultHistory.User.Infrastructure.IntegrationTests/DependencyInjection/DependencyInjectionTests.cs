using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Infrastructure.Database;
using VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;

namespace VaultHistory.User.Infrastructure.IntegrationTests.DependencyInjection;

[Collection(PostgresCollection.Name)]
public sealed class DependencyInjectionTests(PostgresFixture fixture)
{
    [Fact]
    public void AddInfrastructure_RegistersCoreServices()
    {
        using var scope = fixture.ServiceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        var repository = scope.ServiceProvider.GetService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

        Assert.NotNull(dbContext);
        Assert.NotNull(repository);
        Assert.NotNull(unitOfWork);
        Assert.Same(dbContext, unitOfWork);
    }
}
