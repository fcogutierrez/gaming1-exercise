using Domain.Contracts;

namespace Domain.Events;

internal sealed record GuessTooHighEvent(Guid Id, int PlayerId, int Guess) : IDomainEvent;
