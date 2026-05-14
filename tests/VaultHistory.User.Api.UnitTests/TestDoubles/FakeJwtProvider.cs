using VaultHistory.User.Application.Providers.Jwt;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Api.UnitTests.TestDoubles;

internal sealed class FakeJwtProvider(Func<string, Result> validateTokenHandler) : IJwtProvider
{
    public JwtGenerateTokenResult GenerateToken(Domain.Users.User user)
    {
        throw new NotSupportedException();
    }

    public Result ValidateToken(string token)
    {
        return validateTokenHandler(token);
    }
}
