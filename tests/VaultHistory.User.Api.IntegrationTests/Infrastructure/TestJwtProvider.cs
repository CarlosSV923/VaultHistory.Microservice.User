using VaultHistory.User.Application.Providers.Jwt;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Api.IntegrationTests.Infrastructure;

internal sealed class TestJwtProvider : IJwtProvider
{
    public const string ValidToken = "valid-token";

    public JwtGenerateTokenResult GenerateToken(Domain.Users.User user)
    {
        return new JwtGenerateTokenResult(ValidToken, DateTime.UtcNow.AddHours(1));
    }

    public Result ValidateToken(string token)
    {
        return token == ValidToken
            ? Result.Success()
            : Result.Failure(new Error("Jwt.InvalidToken", "Invalid test token"));
    }
}
