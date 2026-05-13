using Microsoft.AspNetCore.Mvc;

namespace VaultHistory.User.Api.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class JwtAuthorizeAttribute : TypeFilterAttribute
    {
        public JwtAuthorizeAttribute()
            : base(typeof(JwtAuthorizationFilter))
        {
        }
    }
}