using System.Collections.ObjectModel;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VaultHistory.User.Api.Extensions
{
    public sealed class SwaggerOptionsSetup(
        IApiVersionDescriptionProvider provider
    ) : IConfigureNamedOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {

            foreach (var description in provider.ApiVersionDescriptions)
            {
                var info = new OpenApiInfo
                {
                    Title = "VaultHistory User API - Version " + description.ApiVersion,
                    Version = description.ApiVersion.ToString(),
                    Description = "API for managing users in the VaultHistory system."
                };

                if (description.IsDeprecated)
                {
                    info.Description += " This API version has been deprecated.";
                }
                options.SwaggerDoc(description.GroupName, info);
            }
        }

        public void Configure(string? name, SwaggerGenOptions options)
        {
            Configure(options);
        }
    }


    public static class Swagger
    {
        public static IServiceCollection AddSwaggerDOC(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.ConfigureOptions<SwaggerOptionsSetup>();
            services.AddSwaggerGen();

            return services;
        }

        public static WebApplication UseSwaggerDoc(this WebApplication app, IReadOnlyCollection<ApiVersionDescription> apiVersionDescriptions)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var groupName in apiVersionDescriptions.Select(description => description.GroupName))
                {
                    options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName.ToUpperInvariant());
                }
            });

            return app;
        }
    }
}