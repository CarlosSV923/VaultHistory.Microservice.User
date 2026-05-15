using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Application.Behaviors;
using VaultHistory.User.Application.Options;
using VaultHistory.User.Application.Providers.Jwt;
using VaultHistory.User.Application.Providers.PasswordHasher;
using VaultHistory.User.Application.Providers.UserContext;

namespace VaultHistory.User.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure options
            services.Configure<PasswordOptions>(configuration.GetSection(PasswordOptions.SectionName));
            services.Configure<HashingOptions>(configuration.GetSection(HashingOptions.SectionName));

            // Providers
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasherProvider, PasswordHasherProvider>();
            services.AddScoped<IUserContextProvider, UserContextProvider>();

            // MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            // Validation
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}