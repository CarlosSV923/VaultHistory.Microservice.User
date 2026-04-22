using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Events;
using VaultHistory.User.Domain.Users.ValueObjects;
using DomainUser = VaultHistory.User.Domain.Users.User;

namespace VaultHistory.User.UnitTests;

public class UserTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var fullNameResult = FullName.Create("Jane", "Doe");
        var emailResult = Email.Create("jane.doe@example.com");
        var passwordResult = Password.Create("hash-value", "salt-value");

        Assert.True(fullNameResult.IsSucceeded);
        Assert.True(emailResult.IsSucceeded);
        Assert.True(passwordResult.IsSucceeded);

        var createData = new CreateUserData(
            fullNameResult.Value,
            emailResult.Value,
            passwordResult.Value,
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)));

        var userResult = DomainUser.Create(createData);

        Assert.True(userResult.IsSucceeded);
        Assert.Equal("jane.doe@example.com", userResult.Value.Email.Value);
        Assert.Contains(userResult.Value.GetDomainEvents(), x => x is CreateUserEvent);
    }

    [Fact]
    public void Create_ShouldFail_WhenBirthDateIsInFuture()
    {
        var fullName = FullName.Create("Jane", "Doe").Value;
        var email = Email.Create("jane.doe@example.com").Value;
        var password = Password.Create("hash-value", "salt-value").Value;

        var createData = new CreateUserData(
            fullName,
            email,
            password,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        var userResult = DomainUser.Create(createData);

        Assert.True(userResult.IsFailure);
        Assert.Equal(UserErrors.BirthDateCannotBeInFuture, userResult.Error);
    }

    [Fact]
    public void ChangeEmail_ShouldAddDomainEvent_WhenEmailChanges()
    {
        var user = DomainUser.Create(new CreateUserData(
            FullName.Create("Jane", "Doe").Value,
            Email.Create("jane.doe@example.com").Value,
            Password.Create("hash-value", "salt-value").Value)).Value;

        user.ClearDomainEvents();

        var changeResult = user.ChangeEmail(Email.Create("jane.new@example.com").Value);

        Assert.True(changeResult.IsSucceeded);
        Assert.Contains(user.GetDomainEvents(), x => x is UserEmailChangedEvent);
    }
}
