using Domain.Contracts;
using Domain.Model.Entities;

namespace Domain.Events;

internal sealed record PlayersCreatedEvent(Guid AggregateId, List<Player> Players) : IDomainEvent;
