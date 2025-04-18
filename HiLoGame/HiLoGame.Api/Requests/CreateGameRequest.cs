using Application.Commands;

namespace HiLoGame.Api.Requests;

public sealed record CreateGameRequest(int Min, int Max)
{
    public CreateGameCommand ToCommand() =>
        new(Min, Max);
}

public sealed record CreateGameResponse(Guid Id);

public static class CreateGameResponseExtensions
{
    public static IResult ToResponse(this CreateGameCommandResult commandResult) =>
        Results.Ok(new CreateGameResponse(commandResult.Id));
}
