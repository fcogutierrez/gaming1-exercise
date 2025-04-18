namespace Domain.Contracts;

public interface IDomainEvent
{
    public Guid AggregateId { get; }
}
