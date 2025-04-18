using Domain.Contracts;
using Domain.Utils;

namespace Domain.Model.Base;

public abstract class AggregateBase
    : IAggregate
{
    private readonly DomainEventAppliersRegistry _domainEventAppliersRegistry = new();
    private readonly List<IDomainEvent> _changes = [];

    public Guid Id { get; protected set; }
    public long Version { get; private set; } = 0;
    public IEnumerable<IDomainEvent> Changes => _changes;

    public AggregateBase()
    {
        RegisterDomainEventAppliers();
    }

    public AggregateBase(IList<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            ApplyDomainEvent(domainEvent);
        }
    }

    public void Save(IDomainEvent @event)
    {
        _changes.Add(@event);
        Version++;
    }

    protected abstract void RegisterDomainEventAppliers();

    protected void ApplyDomainEvent(IDomainEvent domainEvent)
    {
        var applier = _domainEventAppliersRegistry.FindApplier(domainEvent);
        applier(domainEvent);

        _changes.Add(domainEvent);
    }

    protected void RegisterDomainEventApplier<TDomainEvent>(Action<TDomainEvent> applier)
        where TDomainEvent : IDomainEvent
    {
        _domainEventAppliersRegistry.Register(applier);
    }
}