using Application.Contracts;
using Domain.Model;

namespace Application.Commands;

public sealed record AddPlayersCommand(Guid GameId, IEnumerable<string> PlayerNames) : ICommand;

public sealed record AddPlayersCommandResult(IList<CreatedPlayer> Players, PlayerTurn PlayerTurn, int CurrentRound);
