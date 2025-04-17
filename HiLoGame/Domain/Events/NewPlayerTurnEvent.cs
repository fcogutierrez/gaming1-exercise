using Domain.Contracts;

namespace Domain.Events;

internal sealed record NewPlayerTurnEvent(Guid Id, int PlayerId, int Round) : IDomainEvent;
