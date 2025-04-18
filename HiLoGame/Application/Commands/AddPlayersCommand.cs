using Application.Contracts;

namespace Application.Commands;

public sealed record AddPlayersCommand(Guid GameId, IEnumerable<string> PlayerNames) : ICommand;

public sealed record AddPlayersCommandResult() : ICommandResult;