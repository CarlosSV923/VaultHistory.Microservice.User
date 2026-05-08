
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Application.Providers.Jwt
{
    public interface IJwtProvider
    {
        JwtGenerateTokenResult GenerateToken(Domain.Users.User user);
    }
}