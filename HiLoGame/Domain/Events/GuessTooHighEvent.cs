using Domain.Contracts;
using Domain.Model.ValueObjects;

namespace Domain.Events;

internal sealed record GuessTooHighEvent(Guid Id, int PlayerId, GuessAttempt GuessAttempt) : IGuessEvent;
