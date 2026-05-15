using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VaultHistory.User.Api.Extensions
{
    public static class HealthCheck
    {

        public static void AddHealthCheck(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("VaultHistory.User.Api", () => HealthCheckResult.Healthy("The service is healthy!"));
        }

        public static void UseHealthCheck(this WebApplication app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    await context.Response.WriteAsync("Welcome Mr. Stark, the service is healthy!");
                }
            });
        }
    }
}