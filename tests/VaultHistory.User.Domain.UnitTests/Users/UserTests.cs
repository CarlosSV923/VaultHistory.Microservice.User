using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Events;
using VaultHistory.User.Domain.Users.ValueObjects;
using DomainUser = VaultHistory.User.Domain.Users.User;

namespace VaultHistory.User.Domain.UnitTests.Users;

public sealed class UserTests
{
    [Fact]
    public void Create_ShouldFail_WhenDataIsNull()
    {
        var result = DomainUser.Create(null!);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DataRequired, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenBirthDateIsInFuture()
    {
        var data = CreateUserData(birthDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        var result = DomainUser.Create(data);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.BirthDateCannotBeInFuture, result.Error);
    }

    [Fact]
    public void Create_ShouldInitializeUserAndRaiseCreateEvent_WhenDataIsValid()
    {
        var birthDate = new DateOnly(1990, 10, 3);
        var data = CreateUserData(birthDate);

        var result = DomainUser.Create(data);

        Assert.True(result.IsSucceeded);
        Assert.Equal(data.FullName, result.Value.FullName);
        Assert.Equal(data.Email, result.Value.Email);
        Assert.Equal(data.Password, result.Value.Password);
        Assert.Equal(birthDate, result.Value.BirthDate);
        Assert.True(result.Value.IsActive);
        Assert.NotEqual(Guid.Empty, result.Value.Id.Value);

        var domainEvents = result.Value.GetDomainEvents();
        var createEvent = Assert.IsType<CreateUserEvent>(Assert.Single(domainEvents));
        Assert.Equal(result.Value.Id, createEvent.UserId);
    }

    [Fact]
    public void ChangeFullName_ShouldUpdateStateAndRaiseEvent_WhenValueChanges()
    {
        var user = CreateValidUser();
        var newFullName = FullName.Create("Jane", "Smith").Value;
        user.ClearDomainEvents();

        var result = user.ChangeFullName(newFullName);

        Assert.True(result.IsSucceeded);
        Assert.Equal(newFullName, user.FullName);
        Assert.NotNull(user.UpdatedAt);

        var domainEvents = user.GetDomainEvents();
        var changedEvent = Assert.IsType<UserFullNameChangedEvent>(Assert.Single(domainEvents));
        Assert.Equal(user.Id, changedEvent.UserId);
    }

    [Fact]
    public void ChangeFullName_ShouldDoNothing_WhenValueDoesNotChange()
    {
        var user = CreateValidUser();
        var originalUpdatedAt = user.UpdatedAt;
        user.ClearDomainEvents();

        var result = user.ChangeFullName(user.FullName);

        Assert.True(result.IsSucceeded);
        Assert.Equal(originalUpdatedAt, user.UpdatedAt);
        Assert.Empty(user.GetDomainEvents());
    }

    [Fact]
    public void Update_ShouldChangeFullNameAndBirthDate_WhenRequested()
    {
        var user = CreateValidUser();
        var newFullName = FullName.Create("Jane", "Smith").Value;
        var newBirthDate = new DateOnly(1988, 2, 1);
        user.ClearDomainEvents();

        var result = user.Update(new UpdateUserData(newFullName, true, newBirthDate));

        Assert.True(result.IsSucceeded);
        Assert.Equal(newFullName, user.FullName);
        Assert.Equal(newBirthDate, user.BirthDate);
        Assert.NotNull(user.UpdatedAt);

        var domainEvents = user.GetDomainEvents();
        Assert.Equal(2, domainEvents.Count);
        Assert.Contains(domainEvents, domainEvent => domainEvent is UserFullNameChangedEvent);
        Assert.Contains(domainEvents, domainEvent => domainEvent is UserBirthDateChangedEvent);
    }

    [Fact]
    public void Deactivate_ShouldChangeStateAndRaiseEvent_WhenUserIsActive()
    {
        var user = CreateValidUser();
        user.ClearDomainEvents();

        user.Deactivate();

        Assert.False(user.IsActive);
        Assert.NotNull(user.UpdatedAt);

        var domainEvents = user.GetDomainEvents();
        var deactivatedEvent = Assert.IsType<UserDeactivatedEvent>(Assert.Single(domainEvents));
        Assert.Equal(user.Id, deactivatedEvent.UserId);
    }

    [Fact]
    public void Activate_ShouldChangeStateAndRaiseEvent_WhenUserIsInactive()
    {
        var user = CreateValidUser();
        user.Deactivate();
        user.ClearDomainEvents();

        user.Activate();

        Assert.True(user.IsActive);
        Assert.NotNull(user.UpdatedAt);

        var domainEvents = user.GetDomainEvents();
        var activatedEvent = Assert.IsType<UserActivatedEvent>(Assert.Single(domainEvents));
        Assert.Equal(user.Id, activatedEvent.UserId);
    }

    [Fact]
    public void Activate_ShouldDoNothing_WhenUserIsAlreadyActive()
    {
        var user = CreateValidUser();
        var originalUpdatedAt = user.UpdatedAt;
        user.ClearDomainEvents();

        user.Activate();

        Assert.True(user.IsActive);
        Assert.Equal(originalUpdatedAt, user.UpdatedAt);
        Assert.Empty(user.GetDomainEvents());
    }

    [Fact]
    public void RecordSignIn_ShouldRaiseEventWithTheUserAndUtcOccurrenceTime()
    {
        var user = CreateValidUser();
        user.ClearDomainEvents();
        var occurredOn = new DateTime(2026, 9, 7, 14, 30, 0, DateTimeKind.Utc);

        user.RecordSignIn(occurredOn);

        var signedInEvent = Assert.IsType<UserSignedInEvent>(Assert.Single(user.GetDomainEvents()));
        Assert.Equal(user.Id, signedInEvent.UserId);
        Assert.Equal(occurredOn, signedInEvent.OccurredOn);
        Assert.NotNull(user.UpdatedAt);
    }

    private static DomainUser CreateValidUser(DateOnly? birthDate = null)
    {
        return DomainUser.Create(CreateUserData(birthDate)).Value;
    }

    private static CreateUserData CreateUserData(DateOnly? birthDate = null)
    {
        return new CreateUserData(
            FullName.Create("John", "Doe").Value,
            Email.Create("john.doe@example.com").Value,
            Password.Create("hash", "salt").Value,
            birthDate);
    }
}
