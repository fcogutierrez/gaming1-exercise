using Domain.Contracts;

namespace Domain.Events;

internal sealed record NewPlayerTurnEvent(Guid Id, Guid PlayerId, int Round) : IDomainEvent;
