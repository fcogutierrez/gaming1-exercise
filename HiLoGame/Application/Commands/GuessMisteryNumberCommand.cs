using Application.Contracts;

namespace Application.Commands;

public sealed record GuessMisteryNumberCommand(Guid GameId, Guid PlayerId, int Guess) : ICommand;

public sealed record GuessMisteryNumberCommandResult() : ICommandResult;
