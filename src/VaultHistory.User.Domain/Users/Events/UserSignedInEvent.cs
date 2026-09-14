using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users.Events
{
    public record UserSignedInEvent(UserId UserId, DateTime OccurredOn) : IDomainEvent;
}
