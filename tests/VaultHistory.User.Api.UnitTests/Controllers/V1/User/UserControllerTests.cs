using MediatR;
using Microsoft.AspNetCore.Mvc;
using VaultHistory.User.Api.Controllers.V1.User;
using VaultHistory.User.Api.UnitTests.TestDoubles;
using VaultHistory.User.Application.UseCases.GetUserById;
using VaultHistory.User.Application.UseCases.SigninUser;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Api.UnitTests.Controllers.V1.User;

public sealed class UserControllerTests
{
    [Fact]
    public async Task Signin_WhenUseCaseSucceeds_ReturnsOkWithMappedResponse()
    {
        var expectedExpiration = new DateTime(2026, 5, 12, 12, 0, 0, DateTimeKind.Utc);
        var mediator = new FakeMediator((request, _) =>
        {
            var useCase = Assert.IsType<SigninUserRequestDto>(request);
            Assert.Equal("user@test.com", useCase.Email);
            Assert.Equal("Passw0rd!", useCase.Password);
            return Result.Success(new SigninUserResponseDto("jwt-token", expectedExpiration));
        });

        var controller = new UserController(mediator, new FakeUserContextProvider());

        var result = await controller.Signin(new SigninRequest("user@test.com", "Passw0rd!"), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<SigninResponse>(ok.Value);
        Assert.Equal("jwt-token", payload.Token);
        Assert.Equal(expectedExpiration, payload.Expiration);
    }

    [Fact]
    public async Task Signin_WhenUseCaseFails_ReturnsBadRequestWithError()
    {
        var mediator = new FakeMediator((request, _) =>
        {
            Assert.IsType<SigninUserRequestDto>(request);
            return Result.Failure<SigninUserResponseDto>(new Error("Signin.InvalidCredentials", "Credenciales invalidas"));
        });

        var controller = new UserController(mediator, new FakeUserContextProvider());

        var result = await controller.Signin(new SigninRequest("user@test.com", "wrong"), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<Error>(badRequest.Value);
        Assert.Equal("Signin.InvalidCredentials", payload.Code);
        Assert.Equal("Credenciales invalidas", payload.Message);
    }

    [Fact]
    public async Task GetById_WhenUseCaseSucceeds_ReturnsOkWithMappedResponse()
    {
        var birthDate = new DateOnly(1990, 7, 10);
        var mediator = new FakeMediator((request, _) =>
        {
            var useCase = Assert.IsType<GetUserByIdRequestDto>(request);
            Assert.Equal("u-123", useCase.UserId);

            return Result.Success(new GetUserByIdResponseDto(
                UserId: "u-123",
                FirstName: "Ana",
                LastName: "Diaz",
                Email: "ana@test.com",
                BirthDate: birthDate,
                IsActive: true
            ));
        });

        var controller = new UserController(mediator, new FakeUserContextProvider("u-123"));

        var result = await controller.GetById(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<GetByIdResponse>(ok.Value);
        Assert.Equal("u-123", payload.Id);
        Assert.Equal("Ana", payload.FirstName);
        Assert.Equal("Diaz", payload.LastName);
        Assert.Equal("ana@test.com", payload.Email);
        Assert.Equal(birthDate, payload.BirthDate);
        Assert.True(payload.IsActive);
    }

    [Fact]
    public async Task GetById_WhenUseCaseFails_ReturnsBadRequestWithError()
    {
        var mediator = new FakeMediator((request, _) =>
        {
            Assert.IsType<GetUserByIdRequestDto>(request);
            return Result.Failure<GetUserByIdResponseDto>(new Error("User.NotFound", "User not found."));
        });

        var controller = new UserController(mediator, new FakeUserContextProvider("missing"));

        var result = await controller.GetById(CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<Error>(badRequest.Value);
        Assert.Equal("User.NotFound", payload.Code);
        Assert.Equal("User not found.", payload.Message);
    }
}
