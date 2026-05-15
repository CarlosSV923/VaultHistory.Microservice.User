using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VaultHistory.User.Application.Providers.Jwt;

namespace VaultHistory.User.Api.Security
{
    public sealed class JwtAuthorizationFilter(
            ILogger<JwtAuthorizationFilter> logger,
            IJwtProvider jwtProvider
    )
    : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authorizationHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
            var problemDetails = new ProblemDetails
            {
              Title = "Unauthorized",
              Status = StatusCodes.Status401Unauthorized,  
              Type = "Unauthorized",
              Instance = context.HttpContext.Request.Path
            };

            var objResult = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
            {
                logger.LogWarning("Authorization header missing or does not start with 'Bearer '");
                problemDetails.Detail = "Authorization header is required and must start with 'Bearer '.";
                context.Result = objResult;
                return;
            }

            var token = authorizationHeader["Bearer ".Length..].Trim();

            var validationResult = jwtProvider.ValidateToken(token);

            if (validationResult.IsFailure)
            {
                logger.LogWarning("Invalid JWT token: {Reason}", validationResult.Error.Message);
                problemDetails.Detail = validationResult.Error.Message;
                context.Result = objResult;
            }

            context.HttpContext.User = validationResult.Value;
        }
    }
}