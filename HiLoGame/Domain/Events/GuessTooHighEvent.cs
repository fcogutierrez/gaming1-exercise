using Domain.Contracts;
using Domain.Model.ValueObjects;

namespace Domain.Events;

internal sealed record GuessTooHighEvent(Guid AggregateId, Guid PlayerId, GuessAttempt GuessAttempt) : IGuessEvent;
