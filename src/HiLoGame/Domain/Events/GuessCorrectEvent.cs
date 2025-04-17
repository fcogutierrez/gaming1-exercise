using Domain.Contracts;

namespace Domain.Events;

internal sealed record GuessCorrectEvent(Guid Id, int PlayerId, int Guess) : IDomainEvent;
