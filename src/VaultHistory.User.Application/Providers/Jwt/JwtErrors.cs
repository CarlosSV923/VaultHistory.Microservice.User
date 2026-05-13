using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Application.Providers.Jwt
{
    public static class JwtErrors
    {
        public static readonly Error InvalidToken = new("Jwt.InvalidToken", "The provided JWT token is invalid.");
        public static readonly Error TokenExpired = new("Jwt.TokenExpired", "The provided JWT token has expired.");
        public static readonly Error InvalidSignature = new("Jwt.InvalidSignature", "The provided JWT token has an invalid signature.");
        public static readonly Error InvalidIssuer = new("Jwt.InvalidIssuer", "The provided JWT token has an invalid issuer.");
        public static readonly Error InvalidAudience = new("Jwt.InvalidAudience", "The provided JWT token has an invalid audience.");
        public static readonly Error MalformedToken = new("Jwt.MalformedToken", "The provided JWT token is malformed.");
    }
}