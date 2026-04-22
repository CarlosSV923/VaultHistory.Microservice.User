using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users.Events
{
    public record UserPasswordChangedEvent(UserId UserId) : IDomainEvent;
}
