using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VaultHistory.User.Api.Controllers.V1.User;
using VaultHistory.User.Application.Providers.Jwt;

namespace VaultHistory.User.Api.IntegrationTests.Infrastructure;

public sealed class ApiWebApplicationFactory(string connectionString) : WebApplicationFactory<UserController>
{
    private readonly string _connectionString = connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString,
                ["Hashing:MemorySize"] = "65536",
                ["Hashing:DegreeOfParallelism"] = "4",
                ["Hashing:Iterations"] = "3",
                ["Hashing:SaltSize"] = "16",
                ["Hashing:HashSize"] = "32",
                ["Password:MinLength"] = "8",
                ["Password:Regex"] = "",
                ["Jwt:PrivateKey"] = "ignored-in-tests",
                ["Jwt:Issuer"] = "VaultHistory.User.Api.Tests",
                ["Jwt:Audience"] = "VaultHistory.User.Clients.Tests",
                ["Jwt:ExpirationMinutes"] = "30"
            };

            configBuilder.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IJwtProvider>();
            services.AddSingleton<IJwtProvider, TestJwtProvider>();
        });
    }
}
