using Domain.Contracts;
using Domain.Model.ValueObjects;

namespace Domain.Events;

internal sealed record GuessTooHighEvent(Guid Id, Guid PlayerId, GuessAttempt GuessAttempt) : IGuessEvent;
