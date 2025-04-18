using Application.Commands;
using Domain.Model;

namespace HiLoGame.Api.Requests;

public sealed record AddPlayersRequest(IEnumerable<string> PlayerNames)
{
    public AddPlayersCommand ToCommand(Guid gameId) =>
        new(gameId, PlayerNames);
}

public sealed record AddPlayersResponse(PlayerTurnResponse PlayerTurn, int CurrentRound);

public sealed record PlayerTurnResponse(Guid Id, int Order, string Name);

public static class AddPlayersResponseExtensions
{
    public static IResult ToResponse(this AddPlayersCommandResult commandResult) =>
        Results.Ok(new AddPlayersResponse(commandResult.PlayerTurn.ToResponse(), commandResult.CurrentRound));

    private static PlayerTurnResponse ToResponse(this PlayerTurn playerTurn) =>
        new PlayerTurnResponse(playerTurn.Id, playerTurn.Order, playerTurn.Name);
}

