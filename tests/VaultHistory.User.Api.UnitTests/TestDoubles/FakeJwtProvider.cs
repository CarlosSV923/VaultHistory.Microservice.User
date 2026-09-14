using System.Security.Claims;
using VaultHistory.User.Application.Providers.Jwt;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Api.UnitTests.TestDoubles;

internal sealed class FakeJwtProvider(Func<string, Result<ClaimsPrincipal>> validateTokenHandler) : IJwtProvider
{
    public JwtGenerateTokenResult GenerateToken(Domain.Users.User user)
    {
        throw new NotSupportedException();
    }

    public Result<ClaimsPrincipal> ValidateToken(string token)
    {
        return validateTokenHandler(token);
    }
}
