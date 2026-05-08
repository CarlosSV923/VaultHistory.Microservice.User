using VaultHistory.User.Api.Middlewares;

namespace VaultHistory.User.Api.Extensions
{
    public static class Middlewares
    {
        public static void AddMiddlewares(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ExceptionHandler>();
        }

        public static void UseMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<ExceptionHandler>();
        }
    }
}