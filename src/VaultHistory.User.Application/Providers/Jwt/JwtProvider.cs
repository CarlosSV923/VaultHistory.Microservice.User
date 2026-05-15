using Microsoft.Extensions.Options;
using VaultHistory.User.Application.Options;
using VaultHistory.User.Domain.Abstractions;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace VaultHistory.User.Application.Providers.Jwt
{
    public sealed class JwtProvider(
        IOptions<JwtOptions> options
    ) : IJwtProvider
    {
        private readonly JwtOptions _options = options.Value;

        public JwtGenerateTokenResult GenerateToken(Domain.Users.User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.Value),
                new(JwtRegisteredClaimNames.Name, $"{user.FullName.GetFullName()}"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.PrivateKey));
            var expiration = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtGenerateTokenResult(
                new JwtSecurityTokenHandler().WriteToken(token),
                expiration
            );

        }

        public Result<ClaimsPrincipal> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.PrivateKey));

            try
            {
                var claimsPrincipals = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true
                }, out _);
                return Result.Success(claimsPrincipals);
            }
            catch (SecurityTokenExpiredException)
            {
                return Result.Failure<ClaimsPrincipal>(JwtErrors.TokenExpired);
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return Result.Failure<ClaimsPrincipal>(JwtErrors.InvalidSignature);
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                return Result.Failure<ClaimsPrincipal>(JwtErrors.InvalidIssuer);
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                return Result.Failure<ClaimsPrincipal>(JwtErrors.InvalidAudience);
            }
            catch (SecurityTokenMalformedException)
            {
                return Result.Failure<ClaimsPrincipal>(JwtErrors.MalformedToken);
            }
            catch (Exception)
            {
                return Result.Failure<ClaimsPrincipal>(JwtErrors.InvalidToken);
            }
        }
    }
}