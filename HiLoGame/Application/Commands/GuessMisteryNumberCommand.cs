using Application.Contracts;
using Domain.Model;
using OneOf;

namespace Application.Commands;

public sealed record GuessMisteryNumberCommand(Guid GameId, Guid PlayerId, int Guess) : ICommand;

public sealed record NextTurnCommandResult(PlayerTurn PlayerTurn, int CurrentRound, FailedGuessHint PreviousPlayerGuessStatus);

public sealed record MisteryNumberGuessedCommandResult(WinnerPlayer Winner, int Value);