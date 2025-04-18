using Domain.Contracts;
using Domain.Model.ValueObjects;

namespace Domain.Events;

internal sealed record GuessTooLowEvent(Guid Id, Guid PlayerId, GuessAttempt GuessAttempt) : IGuessEvent;   
