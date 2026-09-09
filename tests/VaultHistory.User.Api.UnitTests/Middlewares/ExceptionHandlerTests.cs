using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using VaultHistory.User.Api.Middlewares;
using VaultHistory.User.Application.Exceptions;

namespace VaultHistory.User.Api.UnitTests.Middlewares;

public sealed class ExceptionHandlerTests
{
    [Fact]
    public async Task InvokeAsync_WhenValidationExceptionIsThrown_ReturnsBadRequestProblemDetails()
    {
        var middleware = new ExceptionHandler(
            NullLogger<ExceptionHandler>.Instance,
            _ => throw new ValidationException([
                new ValidateError("Email", "Email invalido")
            ]));

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/user/signup";
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        using var payload = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("Error.ValidationError", payload.RootElement.GetProperty("code").GetString());
        Assert.Equal("One or more validation errors occurred.", payload.RootElement.GetProperty("message").GetString());
        Assert.Equal(JsonValueKind.Array, payload.RootElement.GetProperty("errors").ValueKind);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledExceptionIsThrown_ReturnsInternalServerErrorProblemDetails()
    {
        var middleware = new ExceptionHandler(
            NullLogger<ExceptionHandler>.Instance,
            _ => throw new InvalidOperationException("database exploded"));

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/user/signin";
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        using var payload = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("Error.InternalServerError", payload.RootElement.GetProperty("code").GetString());
        Assert.Equal("An unexpected error occurred. Please try again later.", payload.RootElement.GetProperty("message").GetString());
    }
}
