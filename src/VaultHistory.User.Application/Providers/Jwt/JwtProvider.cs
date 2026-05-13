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

        public JwtGenerateTokenResult GenerateToken(Domain.Users.User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.Value),
                new(JwtRegisteredClaimNames.Name, $"{user.FullName.GetFullName()}"),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_options.PrivateKey));
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

        public Result ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_options.PrivateKey));

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true
                }, out _);


                return Result.Success();
            }
            catch (SecurityTokenExpiredException)
            {
                return Result.Failure(JwtErrors.TokenExpired);
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return Result.Failure(JwtErrors.InvalidSignature);
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                return Result.Failure(JwtErrors.InvalidIssuer);
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                return Result.Failure(JwtErrors.InvalidAudience);
            }
            catch (SecurityTokenMalformedException)
            {
                return Result.Failure(JwtErrors.MalformedToken);
            }
            catch (Exception)
            {
                return Result.Failure(JwtErrors.InvalidToken);
            }
        }
    }
}