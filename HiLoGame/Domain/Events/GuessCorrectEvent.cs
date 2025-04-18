using Domain.Contracts;
using Domain.Model.ValueObjects;

namespace Domain.Events;

internal sealed record GuessCorrectEvent(Guid Id, Guid PlayerId, GuessAttempt GuessAttempt) : IGuessEvent;
