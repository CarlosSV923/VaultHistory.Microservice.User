using FluentValidation;
using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Behaviors;
using VaultHistory.User.Application.Exceptions;
using VaultHistory.User.Domain.Abstractions;
using AppValidationException = VaultHistory.User.Application.Exceptions.ValidationException;

namespace VaultHistory.User.Application.UnitTests.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_ShouldCallNext_WhenThereAreNoValidators()
    {
        var behavior = new ValidationBehavior<FakeUseCase, Result>(Array.Empty<IValidator<FakeUseCase>>());
        var nextCalled = false;

        Task<Result> Next(CancellationToken _) 
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        }

        var result = await behavior.Handle(new FakeUseCase("john@example.com"), Next, CancellationToken.None);

        Assert.True(nextCalled);
        Assert.True(result.IsSucceeded);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        var validators = new IValidator<FakeUseCase>[] { new FakeUseCaseValidator() };
        var behavior = new ValidationBehavior<FakeUseCase, Result>(validators);

        async Task<Result> Next(CancellationToken _) => await Task.FromResult(Result.Success());

        var exception = await Assert.ThrowsAsync<AppValidationException>(() =>
            behavior.Handle(new FakeUseCase(string.Empty), Next, CancellationToken.None));

        var error = Assert.Single(exception.Errors);
        Assert.Equal(nameof(FakeUseCase.Email), error.PropertyName);
        Assert.Equal("Email is required", error.ErrorMessage);
    }

    public sealed record FakeUseCase(string Email) : IUseCase;

    private sealed class FakeUseCaseValidator : AbstractValidator<FakeUseCase>
    {
        public FakeUseCaseValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required");
        }
    }
}