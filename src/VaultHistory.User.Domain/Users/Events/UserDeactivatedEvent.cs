using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users.Events
{
    public record UserDeactivatedEvent(UserId UserId) : IDomainEvent;
}
