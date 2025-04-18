using Domain.Contracts;
using Domain.Model.Entities;

namespace Domain.Events;

internal sealed record PlayersAddedEvent(Guid AggregateId, List<Player> Players) : IDomainEvent;
