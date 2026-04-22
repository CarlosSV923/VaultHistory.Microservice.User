namespace VaultHistory.User.Domain.Abstractions
{
    public abstract class Entity<TEntityId> : IEntity
    {
        protected Entity() { }
        protected Entity(TEntityId id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
        private readonly List<IDomainEvent> _domainEvents = [];
        public TEntityId Id { get; init; } = default!;

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
        

        public IReadOnlyList<IDomainEvent> GetDomainEvents()
        {
            return _domainEvents.AsReadOnly();
        }

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}