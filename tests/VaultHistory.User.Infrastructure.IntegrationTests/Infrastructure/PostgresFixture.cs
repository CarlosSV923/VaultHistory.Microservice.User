using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using VaultHistory.User.Infrastructure.Database;

namespace VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;

public sealed class PostgresFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public IServiceProvider ServiceProvider { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        try
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .WithDatabase("vaulthistory_user_tests")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await _container.StartAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Unable to start Testcontainers PostgreSQL. Ensure Docker is running and reachable.",
                ex);
        }

        ServiceProvider = BuildServiceProvider();
        await ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    private ServiceProvider BuildServiceProvider()
    {
        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = _container!.GetConnectionString()
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        return services.BuildServiceProvider();
    }
}
