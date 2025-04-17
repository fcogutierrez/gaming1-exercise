using Domain.Contracts;

namespace Domain.Events;

internal sealed record GuessTooLowEvent(Guid Id, int PlayerId, int Guess) : IDomainEvent;
