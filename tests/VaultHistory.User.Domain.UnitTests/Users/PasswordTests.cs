using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Domain.UnitTests.Users;

public sealed class PasswordTests
{
    [Fact]
    public void Create_ShouldFail_WhenHashIsMissing()
    {
        var result = Password.Create(" ", "salt");

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.PasswordHashRequired, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenSaltIsMissing()
    {
        var result = Password.Create("hash", null);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.PasswordSaltRequired, result.Error);
    }

    [Fact]
    public void Create_ShouldTrimValues_WhenHashAndSaltAreValid()
    {
        var result = Password.Create(" hash ", " salt ");

        Assert.True(result.IsSucceeded);
        Assert.Equal("hash", result.Value.Hash);
        Assert.Equal("salt", result.Value.Salt);
    }
}