using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace VaultHistory.User.Application.Providers.UserContext
{
    public sealed class UserContextProvider(
        IHttpContextAccessor httpContextAccessor
    ) : IUserContextProvider
    {
        private readonly ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;
        public string GetUserId()
        {
            var userIdClaim = user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim?.Value ?? string.Empty;
        }

        public string GetUserEmail()
        {
            var emailClaim = user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            return emailClaim?.Value ?? string.Empty;
        }

        public string GetUserFullName()
        {
            var nameClaim = user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            return nameClaim?.Value ?? string.Empty;
        }
    }
}