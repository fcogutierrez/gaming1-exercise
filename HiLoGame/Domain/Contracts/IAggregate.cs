namespace Domain.Contracts;

public interface IAggregate
{
    public Guid Id { get; }
    public IList<IDomainEvent> Changes { get; }
    public long Version { get; }
}
