using Domain.Contracts;

namespace Domain.Events;

internal sealed record NewRoundStartedEvent(Guid Id, int Round) : IDomainEvent;
