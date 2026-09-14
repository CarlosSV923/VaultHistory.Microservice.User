using VaultHistory.User.Application.Providers.UserContext;

namespace VaultHistory.User.Api.UnitTests.TestDoubles;

internal sealed class FakeUserContextProvider(string userId = "u-123") : IUserContextProvider
{
    public string GetUserId() => userId;

    public string GetUserEmail() => "user@test.com";

    public string GetUserFullName() => "Test User";
}
