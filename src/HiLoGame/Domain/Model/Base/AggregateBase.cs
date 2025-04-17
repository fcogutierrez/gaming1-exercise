using Domain.Contracts;

namespace Domain.Model.Base;

public abstract class AggregateBase
    : IAggregate
{
    public Guid Id { get; protected set; }        
    public long Version { get; private set; }
    public IList<IDomainEvent> Changes { get; }

    public AggregateBase()
    {
        Version = 0;
        Changes = [];
    }

    public void Save(IDomainEvent @event)
    {
        Changes.Add(@event);
        Version++;
    }
}