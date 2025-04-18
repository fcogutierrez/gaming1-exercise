using Domain.Contracts;

namespace Domain.Events;

internal sealed record NewPlayerTurnEvent(Guid AggregateId, Guid PlayerId, int Round) : IDomainEvent;
