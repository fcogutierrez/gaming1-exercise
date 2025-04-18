using Domain.Contracts;

namespace Domain.Utils;

internal sealed class DomainEventAppliersRegistry
{
    private readonly Dictionary<Type, Action<IDomainEvent>> _appliers;

    public DomainEventAppliersRegistry()
    {
        _appliers = [];
    }

    public Action<IDomainEvent> FindApplier(IDomainEvent domainEvent)
    {
        var applierLookupKey = domainEvent.GetType();
        if (!_appliers.TryGetValue(applierLookupKey, out Action<IDomainEvent> applier))
        {
            throw new KeyNotFoundException($"No applier found for event type {applierLookupKey}");
        }

        return applier;
    }

    public void Register<TDomainEvent>(Action<TDomainEvent> applier) 
        where TDomainEvent : IDomainEvent
    {
        var eventType = typeof(TDomainEvent);
        void Applier(IDomainEvent domainEvent) => applier((TDomainEvent)domainEvent);

        _appliers.Add(eventType, Applier);
    }
}
