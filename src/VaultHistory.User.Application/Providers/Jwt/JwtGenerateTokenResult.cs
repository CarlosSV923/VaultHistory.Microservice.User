namespace VaultHistory.User.Application.Providers.Jwt
{
    public record JwtGenerateTokenResult(string Token, DateTime Expiration);
}