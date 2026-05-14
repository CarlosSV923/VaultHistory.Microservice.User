using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;

namespace VaultHistory.User.Infrastructure.IntegrationTests.Repositories.UserRepository;

[Collection(PostgresCollection.Name)]
public sealed class CancellationTests(PostgresFixture fixture)
{
    [Fact]
    public async Task GetByIdAsync_WhenCancellationIsRequested_ThrowsOperationCanceledException()
    {
        await fixture.ResetDatabaseAsync();

        using var scope = fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repository.GetByIdAsync(UserId.NewId(), cancellationTokenSource.Token));
    }

    [Fact]
    public async Task GetByEmailAsync_WhenCancellationIsRequested_ThrowsOperationCanceledException()
    {
        await fixture.ResetDatabaseAsync();

        using var scope = fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var emailResult = Email.Create("cancel@test.com");
        Assert.True(emailResult.IsSucceeded);

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repository.GetByEmailAsync(emailResult.Value, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task VerifyEmailExistsAsync_WhenCancellationIsRequested_ThrowsOperationCanceledException()
    {
        await fixture.ResetDatabaseAsync();

        using var scope = fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var emailResult = Email.Create("cancel@test.com");
        Assert.True(emailResult.IsSucceeded);

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repository.VerifyEmailExistsAsync(emailResult.Value, cancellationTokenSource.Token));
    }
}
