using Application.Contracts;

namespace Application.Commands;

public sealed record CreateGameCommand(int Min, int Max) : ICommand;

public sealed record CreateGameCommandResult() : ICommandResult;
