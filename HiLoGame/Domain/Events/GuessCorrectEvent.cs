using Domain.Contracts;
using Domain.Model.ValueObjects;

namespace Domain.Events;

internal sealed record GuessCorrectEvent(Guid AggregateId, Guid PlayerId, GuessAttempt GuessAttempt) : IGuessEvent;
