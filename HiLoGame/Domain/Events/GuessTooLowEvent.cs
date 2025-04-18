using Domain.Contracts;
using Domain.Model.ValueObjects;

namespace Domain.Events;

internal sealed record GuessTooLowEvent(Guid AggregateId, Guid PlayerId, GuessAttempt GuessAttempt) : IGuessEvent;   
