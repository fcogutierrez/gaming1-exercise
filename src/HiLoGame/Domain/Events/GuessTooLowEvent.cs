using Domain.Contracts;
using Domain.Model.ValueObjects;

namespace Domain.Events;

internal sealed record GuessTooLowEvent(Guid Id, int PlayerId, GuessAttempt GuessAttempt) : IGuessEvent;   
