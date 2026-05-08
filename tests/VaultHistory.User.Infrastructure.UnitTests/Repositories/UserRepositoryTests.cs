using Microsoft.EntityFrameworkCore;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Domain.Users.ValueObjects;
using VaultHistory.User.Infrastructure;
using VaultHistory.User.Infrastructure.Database;

namespace VaultHistory.User.Infrastructure.UnitTests.Repositories;

public sealed class UserRepositoryTests
{
    [Fact]
    public async Task Add_AndGetByIdAsync_ShouldReturnPersistedUser()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);
        var user = CreateUser("john.doe@example.com");

        repository.Add(user);
        await context.SaveChangesAsync();

        var loaded = await repository.GetByIdAsync(user.Id);

        Assert.NotNull(loaded);
        Assert.Equal(user.Id, loaded!.Id);
        Assert.Equal("john.doe@example.com", loaded.Email.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);

        var loaded = await repository.GetByIdAsync(UserId.NewId());

        Assert.Null(loaded);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);
        var user = CreateUser("jane.doe@example.com");

        repository.Add(user);
        await context.SaveChangesAsync();

        var loaded = await repository.GetByEmailAsync(Email.Create("jane.doe@example.com").Value);

        Assert.NotNull(loaded);
        Assert.Equal(user.Id, loaded!.Id);
    }

    [Fact]
    public async Task VerifyEmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);
        var user = CreateUser("exists@example.com");

        repository.Add(user);
        await context.SaveChangesAsync();

        var exists = await repository.VerifyEmailExistsAsync(Email.Create("exists@example.com").Value);

        Assert.True(exists);
    }

    [Fact]
    public async Task VerifyEmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        await using var context = CreateContext();
        var repository = CreateRepository(context);

        var exists = await repository.VerifyEmailExistsAsync(Email.Create("missing@example.com").Value);

        Assert.False(exists);
    }

    [Fact]
    public async Task Update_ShouldPersistChanges_WhenEntityIsModified()
    {
        var dbName = $"user-repo-{Guid.NewGuid()}";

        await using (var arrangeContext = CreateContext(dbName))
        {
            var repository = CreateRepository(arrangeContext);
            var user = CreateUser("before@example.com");

            repository.Add(user);
            await arrangeContext.SaveChangesAsync();

            var changedEmail = Email.Create("after@example.com").Value;
            user.ChangeEmail(changedEmail);
            repository.Update(user);
            await arrangeContext.SaveChangesAsync();
        }

        await using (var assertContext = CreateContext(dbName))
        {
            var repository = CreateRepository(assertContext);

            var loaded = await repository.GetByEmailAsync(Email.Create("after@example.com").Value);

            Assert.NotNull(loaded);
            Assert.Equal("after@example.com", loaded!.Email.Value);
        }
    }

    private static ApplicationDbContext CreateContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? $"user-repo-{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private static IUserRepository CreateRepository(ApplicationDbContext context)
    {
        var repositoryType = typeof(DependencyInjection).Assembly
            .GetType("VaultHistory.User.Infrastructure.Repositories.UserRepository", throwOnError: true)!;

        return (IUserRepository)Activator.CreateInstance(repositoryType, context)!;
    }

    private static Domain.Users.User CreateUser(string email)
    {
        var createData = new CreateUserData(
            FullName.Create("John", "Doe").Value,
            Email.Create(email).Value,
            Password.Create("hash", "salt").Value,
            new DateOnly(1990, 1, 1));

        return Domain.Users.User.Create(createData).Value;
    }
}