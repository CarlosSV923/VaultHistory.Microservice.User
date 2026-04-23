using Microsoft.Extensions.Options;
using VaultHistory.User.Application.Options;
using VaultHistory.User.Domain.Abstractions;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace VaultHistory.User.Application.Providers.Jwt
{
    public sealed class JwtProvider(
        IOptions<JwtOptions> options
    ) : IJwtProvider
    {
        private readonly JwtOptions _options = options.Value;

        public Result<JwtGenerateTokenResult> GenerateToken(Domain.Users.User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.Value),
                new(JwtRegisteredClaimNames.Name, $"{user.FullName.GetFullName()}"),
            };
            try
            {
                using var rsa = RSA.Create();
                rsa.ImportFromPem(_options.PrivateKey.ToCharArray());
                var key = new RsaSecurityKey(rsa);
                var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

                var expiration = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);
                var token = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: creds
                );

                return new JwtGenerateTokenResult(
                    new JwtSecurityTokenHandler().WriteToken(token),
                    expiration
                );
            }
            catch (Exception)
            {
                return Result.Failure<JwtGenerateTokenResult>(Error.BuildInternalError("TokenGenerationFailed", $"Failed to generate auth token"));
            }
        }
    }
}