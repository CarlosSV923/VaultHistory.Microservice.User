using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users.Events
{
    public record UserBirthDateChangedEvent(UserId UserId) : IDomainEvent;
}
