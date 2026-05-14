using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using VaultHistory.User.Api.Security;
using VaultHistory.User.Api.UnitTests.TestDoubles;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Api.UnitTests.Security;

public sealed class JwtAuthorizationFilterTests
{
    [Fact]
    public async Task OnAuthorizationAsync_WhenAuthorizationHeaderIsMissing_ReturnsUnauthorized()
    {
        var filter = new JwtAuthorizationFilter(NullLogger<JwtAuthorizationFilter>.Instance, new FakeJwtProvider(_ => Result.Success()));
        var context = CreateAuthorizationFilterContext();

        await filter.OnAuthorizationAsync(context);

        var objectResult = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status401Unauthorized, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("Authorization header is required and must start with 'Bearer '.", problemDetails.Detail);
    }

    [Fact]
    public async Task OnAuthorizationAsync_WhenTokenIsInvalid_ReturnsUnauthorizedWithFailureReason()
    {
        var filter = new JwtAuthorizationFilter(
            NullLogger<JwtAuthorizationFilter>.Instance,
            new FakeJwtProvider(_ => Result.Failure(new Error("Jwt.InvalidToken", "Token invalido"))));
        var context = CreateAuthorizationFilterContext();
        context.HttpContext.Request.Headers.Authorization = "Bearer invalid-token";

        await filter.OnAuthorizationAsync(context);

        var objectResult = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status401Unauthorized, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("Token invalido", problemDetails.Detail);
    }

    [Fact]
    public async Task OnAuthorizationAsync_WhenTokenIsValid_DoesNotSetResult()
    {
        var filter = new JwtAuthorizationFilter(NullLogger<JwtAuthorizationFilter>.Instance, new FakeJwtProvider(_ => Result.Success()));
        var context = CreateAuthorizationFilterContext();
        context.HttpContext.Request.Headers.Authorization = "Bearer valid-token";

        await filter.OnAuthorizationAsync(context);

        Assert.Null(context.Result);
    }

    private static AuthorizationFilterContext CreateAuthorizationFilterContext()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, []);
    }
}
