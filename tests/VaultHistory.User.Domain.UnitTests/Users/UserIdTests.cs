using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Domain.UnitTests.Users;

public sealed class UserIdTests
{
    [Fact]
    public void FromString_ShouldFail_WhenValueIsInvalid()
    {
        var result = UserId.FromString("invalid-guid");

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidUserId, result.Error);
    }

    [Fact]
    public void FromString_ShouldReturnUserId_WhenValueIsValid()
    {
        var id = Guid.NewGuid();

        var result = UserId.FromString(id.ToString());

        Assert.True(result.IsSucceeded);
        Assert.Equal(id, result.Value.Value);
        Assert.Equal(id.ToString(), result.Value.ToString());
    }
}