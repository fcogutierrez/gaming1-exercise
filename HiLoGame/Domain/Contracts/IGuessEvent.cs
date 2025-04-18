using Domain.Model.ValueObjects;

namespace Domain.Contracts;

internal interface IGuessEvent : IDomainEvent
{
    Guid PlayerId { get; }
    GuessAttempt GuessAttempt { get; }
}
