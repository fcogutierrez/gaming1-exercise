namespace Domain.Contracts;

public interface IDomainEvent
{
    public Guid Id { get; }
}
