using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Api.Controllers.V1.User;
using VaultHistory.User.Api.IntegrationTests.Infrastructure;
using VaultHistory.User.Infrastructure.Database;
using VaultHistory.User.Infrastructure.Outbox;
using DomainUser = VaultHistory.User.Domain.Users.User;

namespace VaultHistory.User.Api.IntegrationTests.Controllers.V1.User;

[Collection(ApiIntegrationCollection.Name)]
public sealed class AuthEndpointsTests(ApiIntegrationFixture fixture)
{
    [Fact]
    public async Task Signup_WhenRequestIsValid_ReturnsOkWithToken()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();

        var request = new SignupRequest("Ana", "Diaz", "ana.integration@test.com", "Passw0rd!", null);

        var response = await client.PostAsJsonAsync("/api/v1/User/signup", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<SignupResponse>();
        Assert.NotNull(payload);
        Assert.Equal(TestJwtProvider.ValidToken, payload!.Token);
    }

    [Fact]
    public async Task Signin_WhenCredentialsAreValid_ReturnsOkWithToken()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();

        var signup = new SignupRequest("John", "Doe", "john.integration@test.com", "Passw0rd!", null);
        var signupResponse = await client.PostAsJsonAsync("/api/v1/User/signup", signup);
        Assert.Equal(HttpStatusCode.OK, signupResponse.StatusCode);

        var signin = new SigninRequest("john.integration@test.com", "Passw0rd!");

        var response = await client.PostAsJsonAsync("/api/v1/User/signin", signin);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<SigninResponse>();
        Assert.NotNull(payload);
        Assert.Equal(TestJwtProvider.ValidToken, payload!.Token);

        using var scope = fixture.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var outbox = await dbContext.Set<OutboxMessage>()
            .SingleAsync(message => message.Type == "UserSignedInEvent");
        var signedInUser = await dbContext.Set<DomainUser>()
            .SingleAsync(user => user.Email.Value == "john.integration@test.com");

        Assert.Equal("PENDING", outbox.Status);
        using var outboxPayload = JsonDocument.Parse(outbox.Payload);
        Assert.Equal(JsonValueKind.String, outboxPayload.RootElement.GetProperty("userId").ValueKind);
        Assert.Equal(signedInUser.Id.Value.ToString(), outboxPayload.RootElement.GetProperty("userId").GetString());
        Assert.False(outboxPayload.RootElement.TryGetProperty("$type", out _));
        Assert.DoesNotContain("password", outbox.Payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Signin_WhenEmailFormatIsInvalid_ReturnsBadRequestProblemDetails()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();

        var request = new SigninRequest("invalid-email", "Passw0rd!");

        var response = await client.PostAsJsonAsync("/api/v1/User/signin", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var content = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(400, content.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("ValidationFailure", content.RootElement.GetProperty("type").GetString());
    }

    [Fact]
    public async Task Signin_WhenPasswordIsInvalid_DoesNotCreateASignInOutboxMessage()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();
        await client.PostAsJsonAsync("/api/v1/User/signup", new SignupRequest("Ana", "Diaz", "ana.password@test.com", "Passw0rd!", null));

        var response = await client.PostAsJsonAsync("/api/v1/User/signin", new SigninRequest("ana.password@test.com", "incorrect"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var scope = fixture.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.False(await dbContext.Set<OutboxMessage>().AnyAsync(message => message.Type == "UserSignedInEvent"));
    }

    [Fact]
    public async Task Signin_WhenUserIsInactive_DoesNotCreateASignInOutboxMessage()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();
        await client.PostAsJsonAsync("/api/v1/User/signup", new SignupRequest("Ana", "Diaz", "ana.inactive@test.com", "Passw0rd!", null));

        using (var scope = fixture.Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await dbContext.Set<DomainUser>().SingleAsync(candidate => candidate.Email.Value == "ana.inactive@test.com");
            user.Deactivate();
            await dbContext.SaveChangesAsync();
        }

        var response = await client.PostAsJsonAsync("/api/v1/User/signin", new SigninRequest("ana.inactive@test.com", "Passw0rd!"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var verificationScope = fixture.Factory.Services.CreateScope();
        var verificationContext = verificationScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.False(await verificationContext.Set<OutboxMessage>().AnyAsync(message => message.Type == "UserSignedInEvent"));
    }
}
