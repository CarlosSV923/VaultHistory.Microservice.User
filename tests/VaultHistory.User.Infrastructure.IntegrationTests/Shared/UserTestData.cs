using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.ValueObjects;

namespace VaultHistory.User.Infrastructure.IntegrationTests.Shared;

internal static class UserTestData
{
    public static Domain.Users.User CreateUser(string email)
    {
        var fullNameResult = FullName.Create("Ana", "Diaz");
        Assert.True(fullNameResult.IsSucceeded);

        var emailResult = Email.Create(email);
        Assert.True(emailResult.IsSucceeded);

        var passwordResult = Password.Create("hash-value", "salt-value");
        Assert.True(passwordResult.IsSucceeded);

        var userResult = Domain.Users.User.Create(new CreateUserData(
            fullNameResult.Value,
            emailResult.Value,
            passwordResult.Value,
            new DateOnly(1990, 1, 1)));

        Assert.True(userResult.IsSucceeded);
        return userResult.Value;
    }
}
