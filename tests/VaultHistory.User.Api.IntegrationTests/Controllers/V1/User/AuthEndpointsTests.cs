using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using VaultHistory.User.Api.Controllers.V1.User;
using VaultHistory.User.Api.IntegrationTests.Infrastructure;

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
}
