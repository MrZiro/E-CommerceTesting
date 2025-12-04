using MyCommerce.Domain.Common.Events;

namespace MyCommerce.Domain.Common;

public abstract class AggregateRoot : Entity, IAuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? UpdatedOnUtc { get; set; }

    protected AggregateRoot(Guid id) : base(id) { }
    protected AggregateRoot() { } // For EF Core

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
