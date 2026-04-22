namespace VaultHistory.User.Domain.Abstractions
{
    public interface IEntity
    {
        IReadOnlyList<IDomainEvent> GetDomainEvents();
        void ClearDomainEvents();
    }
}