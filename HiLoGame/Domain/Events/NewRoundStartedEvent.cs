using Domain.Contracts;

namespace Domain.Events;

internal sealed record NewRoundStartedEvent(Guid AggregateId, int Round) : IDomainEvent;
