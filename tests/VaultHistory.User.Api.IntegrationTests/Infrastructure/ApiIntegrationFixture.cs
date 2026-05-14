using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using VaultHistory.User.Infrastructure.Database;

namespace VaultHistory.User.Api.IntegrationTests.Infrastructure;

public sealed class ApiIntegrationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("vaulthistory_api_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public ApiWebApplicationFactory Factory { get; private set; } = default!;

    public HttpClient CreateClient()
    {
        return Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        Factory = new ApiWebApplicationFactory(_container.GetConnectionString());
        await ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        Factory.Dispose();
        await _container.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }
}
