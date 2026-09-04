using VaultHistory.User.Api.Extensions;
using VaultHistory.User.Application;
using VaultHistory.User.Infrastructure;
using Microsoft.EntityFrameworkCore;
using VaultHistory.User.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Configurations/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Configurations/appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

Console.WriteLine($"Current Environment: {environment}");

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerDOC();
builder.Services.AddHealthCheck();
builder.AddAuth();

var app = builder.Build();

// User is the sole owner of the shared users/outbox schema. Jobs only consumes
// these tables through Prisma and therefore never runs schema migrations.
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseMiddlewares();

if (!app.Environment.IsProduction())
{
    app.UseSwaggerDoc(app.DescribeApiVersions());
}

app.UseAuth();
app.UseHealthCheck();
app.MapControllers();
app.UseHttpsRedirection();

await app.RunAsync();


