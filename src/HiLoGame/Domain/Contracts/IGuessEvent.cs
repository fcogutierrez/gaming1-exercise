using Domain.Model.ValueObjects;

namespace Domain.Contracts;

internal interface IGuessEvent : IDomainEvent
{
    int PlayerId { get; }
    GuessAttempt GuessAttempt { get; }
}
