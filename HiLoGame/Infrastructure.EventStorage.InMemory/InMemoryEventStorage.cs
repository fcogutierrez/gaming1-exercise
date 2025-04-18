using Application.Contracts;
using Domain.Contracts;
using System.Collections.Concurrent;

namespace Infrastructure.EventStorage.InMemory;

internal sealed class InMemoryEventStorage : IEventStorage
{
    private readonly ConcurrentDictionary<Guid, List<IDomainEvent>> _storage = new();

    public Task SaveMany(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            var list = _storage.GetOrAdd(@event.AggregateId, _ => []);
            lock (list)
            {
                list.Add(@event);
            }
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<IDomainEvent>> GetByAggregateId(Guid aggregateId)
    {
        if (_storage.TryGetValue(aggregateId, out var events))
        {
            lock (events)
            {
                return Task.FromResult(events.ToList().AsEnumerable());
            }
        }

        return Task.FromResult(Enumerable.Empty<IDomainEvent>());
    }
}
