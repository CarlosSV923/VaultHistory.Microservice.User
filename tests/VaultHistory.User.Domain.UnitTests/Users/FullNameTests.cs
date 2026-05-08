using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Domain.UnitTests.Users;

public sealed class FullNameTests
{
    [Fact]
    public void Create_ShouldFail_WhenFirstNameIsMissing()
    {
        var result = FullName.Create(null, "Doe");

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.FirstNameRequired, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenLastNameIsMissing()
    {
        var result = FullName.Create("John", " ");

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.LastNameRequired, result.Error);
    }

    [Fact]
    public void Create_ShouldTrimNames_WhenValuesAreValid()
    {
        var result = FullName.Create("  John  ", "  Doe ");

        Assert.True(result.IsSucceeded);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal("John Doe", result.Value.GetFullName());
    }
}