using Domain.Contracts;

namespace Application.Contracts;

public interface IEventStorage
{
    Task SaveMany(IEnumerable<IDomainEvent> events);
    Task<IEnumerable<IDomainEvent>> GetByAggregateId(Guid aggregateId);
}
