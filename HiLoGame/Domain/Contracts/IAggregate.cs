namespace Domain.Contracts;

public interface IAggregate
{
    public Guid Id { get; }
    public long Version { get; }
}
