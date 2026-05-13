using VaultHistory.User.Api.Extensions;
using VaultHistory.User.Application;
using VaultHistory.User.Infrastructure;

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
builder.AddAuth();

var app = builder.Build();

app.UseMiddlewares();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDoc(app.DescribeApiVersions());
}

app.UseAuth();
app.MapControllers();
app.UseHttpsRedirection();

await app.RunAsync();


