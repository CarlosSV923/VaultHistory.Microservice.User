using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaultHistory.User.Api.Controllers.V1.User;
using VaultHistory.User.Api.IntegrationTests.Infrastructure;
using VaultHistory.User.Infrastructure.Database;

namespace VaultHistory.User.Api.IntegrationTests.Controllers.V1.User;

[Collection(ApiIntegrationCollection.Name)]
public sealed class ProtectedEndpointsTests(ApiIntegrationFixture fixture)
{
    [Fact]
    public async Task GetById_WhenAuthorizationHeaderIsMissing_ReturnsUnauthorized()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();

        var response = await client.GetAsync($"/api/v1/User/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenTokenIsInvalid_ReturnsUnauthorized()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");

        var response = await client.GetAsync($"/api/v1/User/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenTokenIsValidAndUserExists_ReturnsOk()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();

        var signup = new SignupRequest("Maria", "Lopez", "maria.integration@test.com", "Passw0rd!", null);
        var signupResponse = await client.PostAsJsonAsync("/api/v1/User/signup", signup);
        Assert.Equal(HttpStatusCode.OK, signupResponse.StatusCode);

        string userId;
        using (var scope = fixture.Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await dbContext.Set<Domain.Users.User>()
                .FirstOrDefaultAsync(u => u.Email.Value == "maria.integration@test.com");

            Assert.NotNull(user);
            userId = user!.Id.ToString();
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwtProvider.ValidToken);

        var response = await client.GetAsync($"/api/v1/User/{userId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<GetByIdResponse>();
        Assert.NotNull(payload);
        Assert.Equal(userId, payload!.Id);
        Assert.Equal("maria.integration@test.com", payload.Email);
    }

    [Fact]
    public async Task GetById_WhenTokenIsValidAndUserIdIsInvalidGuid_ReturnsBadRequestValidationFailure()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwtProvider.ValidToken);

        var response = await client.GetAsync("/api/v1/User/not-a-guid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var content = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(400, content.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("ValidationFailure", content.RootElement.GetProperty("type").GetString());
    }

    [Fact]
    public async Task GetById_WhenTokenIsValidAndUserDoesNotExist_ReturnsBadRequestWithNotFoundError()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwtProvider.ValidToken);

        var response = await client.GetAsync($"/api/v1/User/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var content = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("User.NotFound", content.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task GetByEmail_WhenAuthorizationHeaderIsMissing_ReturnsUnauthorized()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();

        var response = await client.GetAsync("/api/v1/User/by-email?email=ana.integration@test.com");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetByEmail_WhenTokenIsInvalid_ReturnsUnauthorized()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");

        var response = await client.GetAsync("/api/v1/User/by-email?email=ana.integration@test.com");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetByEmail_WhenTokenIsValidAndUserExists_ReturnsOk()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();

        var signup = new SignupRequest("Ana", "Diaz", "ana.integration@test.com", "Passw0rd!", null);
        var signupResponse = await client.PostAsJsonAsync("/api/v1/User/signup", signup);
        Assert.Equal(HttpStatusCode.OK, signupResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwtProvider.ValidToken);

        var response = await client.GetAsync("/api/v1/User/by-email?email=ana.integration@test.com");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<GetByEmailResponse>();
        Assert.NotNull(payload);
        Assert.Equal("ana.integration@test.com", payload!.Email);
        Assert.Equal("Ana", payload.FirstName);
        Assert.Equal("Diaz", payload.LastName);
    }

    [Fact]
    public async Task GetByEmail_WhenTokenIsValidAndUserDoesNotExist_ReturnsBadRequestWithNotFoundError()
    {
        await fixture.ResetDatabaseAsync();
        using var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwtProvider.ValidToken);

        var response = await client.GetAsync("/api/v1/User/by-email?email=missing.integration@test.com");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var content = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("User.NotFound", content.RootElement.GetProperty("code").GetString());
    }
}
