using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Domain.UnitTests.Users;

public sealed class EmailTests
{
    [Fact]
    public void Create_ShouldFail_WhenValueIsMissing()
    {
        var result = Email.Create("   ");

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.EmailRequired, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenValueHasInvalidFormat()
    {
        var result = Email.Create("not-an-email");

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidEmailFormat, result.Error);
    }

    [Fact]
    public void Create_ShouldNormalizeValue_WhenValueIsValid()
    {
        var result = Email.Create("  USER@Example.COM ");

        Assert.True(result.IsSucceeded);
        Assert.Equal("user@example.com", result.Value.Value);
        Assert.Equal("user@example.com", result.Value.ToString());
    }
}