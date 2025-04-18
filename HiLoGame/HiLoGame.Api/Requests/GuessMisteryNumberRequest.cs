using Application.Commands;
using Domain.Model;
using OneOf;

namespace HiLoGame.Api.Requests;

using GuessMisteryNumberCommandResult = OneOf<NextTurnCommandResult, MisteryNumberGuessedCommandResult>;

public sealed record GuessMisteryNumberRequest(int Guess)
{
    public GuessMisteryNumberCommand ToCommand(Guid gameId, Guid playerId) =>
        new(gameId, playerId, Guess);
}

public sealed record NextTurnResponse(PlayerTurnResponse PlayerTurn, int CurrentRound, string PreviousPlayerGuessStatus);

public sealed record MisteryNumberGuessedResponse(WinnerPlayerResponse Winner, int Value);

public sealed record WinnerPlayerResponse(Guid Id);

public static class GuessMisteryNumberResponseExtensions
{
    public static IResult ToResponse(this GuessMisteryNumberCommandResult commandResult) =>
        commandResult.Match(
            nextTurn => Results.Ok(nextTurn.ToResponse()),
            numberGuessed => Results.Ok(numberGuessed.ToResponse()));

    public static NextTurnResponse ToResponse(this NextTurnCommandResult commandResult) =>
        new(commandResult.PlayerTurn.ToResponse(), commandResult.CurrentRound, commandResult.PreviousPlayerGuessStatus.ToString());

    public static MisteryNumberGuessedResponse ToResponse(this MisteryNumberGuessedCommandResult commandResult) =>
        new(commandResult.Winner.ToResponse(), commandResult.Value);


    public static WinnerPlayerResponse ToResponse(this WinnerPlayer winner) =>
        new(winner.Id);
}
