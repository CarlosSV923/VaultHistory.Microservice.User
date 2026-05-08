using VaultHistory.User.Api.Extensions;
using VaultHistory.User.Application;
using VaultHistory.User.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerDOC();

builder.AddMiddlewares();
builder.AddAuth();


var app = builder.Build();


app.UseMiddlewares();
app.UseAuth();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDoc(app.DescribeApiVersions());
}

app.UseHttpsRedirection();


await app.RunAsync();


