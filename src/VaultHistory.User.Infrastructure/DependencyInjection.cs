using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VaultHistory.User.Infrastructure.Database;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Infrastructure.Repositories;
using VaultHistory.User.Domain.Users.Interfaces;

namespace VaultHistory.User.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext PostgreSQL
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString);
            });

            // Unit of Work
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}