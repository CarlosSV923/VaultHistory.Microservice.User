using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users.Events
{
    public record UserFullNameChangedEvent(UserId UserId) : IDomainEvent;
}
