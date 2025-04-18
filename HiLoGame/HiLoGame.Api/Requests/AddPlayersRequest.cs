using Application.Commands;
using Domain.Model;

namespace HiLoGame.Api.Requests;

public sealed record AddPlayersRequest(IEnumerable<string> PlayerNames)
{
    public AddPlayersCommand ToCommand(Guid gameId) =>
        new(gameId, PlayerNames);
}

public sealed record AddPlayersResponse(IEnumerable<CreatedPlayerResponse> Players, PlayerTurnResponse PlayerTurn, int CurrentRound);

public sealed record CreatedPlayerResponse(Guid Id, int Order, string Name);

public sealed record PlayerTurnResponse(Guid Id);

public static class AddPlayersResponseExtensions
{
    public static IResult ToResponse(this AddPlayersCommandResult commandResult) =>
        Results.Ok(new AddPlayersResponse(
            commandResult.Players.Select(p => p.ToResponse()), 
            commandResult.PlayerTurn.ToResponse(), 
            commandResult.CurrentRound));

    public static PlayerTurnResponse ToResponse(this PlayerTurn playerTurn) =>
        new(playerTurn.Id);

    public static CreatedPlayerResponse ToResponse(this CreatedPlayer player) =>
        new(player.Id, player.Order, player.Name);
}

