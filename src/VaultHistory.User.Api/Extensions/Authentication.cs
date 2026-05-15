using VaultHistory.User.Api.Security;
using VaultHistory.User.Application.Options;

namespace VaultHistory.User.Api.Extensions
{

    public static class Authentication
    {
        public static void AddAuth(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<JwtAuthorizationFilter>();
            builder.Services.AddHttpContextAccessor();
        }

        public static void UseAuth(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}