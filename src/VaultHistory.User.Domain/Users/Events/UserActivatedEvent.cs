using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users.Events
{
    public record UserActivatedEvent(UserId UserId) : IDomainEvent;
}
