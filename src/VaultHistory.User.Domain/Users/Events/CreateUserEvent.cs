using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users.Events
{
    public record CreateUserEvent(UserId UserId) : IDomainEvent;
}