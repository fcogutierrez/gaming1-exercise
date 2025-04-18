using Application.Commands;
using Application.Contracts;
using HiLoGame.Api.Requests;
using OneOf;

namespace HiLoGame.Api;

using GuessMisteryNumberCommandResult = OneOf<NextTurnCommandResult, MisteryNumberGuessedCommandResult>;

internal static class Endpoints
{
    public static IEndpointRouteBuilder UseHiLoGameEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/games/hilo");

        api.MapPost(
            "/",
            async (
                CreateGameRequest request,
                ICommandHandler<CreateGameCommand, CreateGameCommandResult> commandHandler
            ) =>
            {
                var command = request.ToCommand();
                var commandResult = await commandHandler.Handle(command);

                return commandResult.ToResponse();
            }).WithName(EndpointNames.CreateGame);

        api.MapPost(
            "/{gameId}/players",
            async(
                Guid gameId,
                AddPlayersRequest request,
                ICommandHandler<AddPlayersCommand, AddPlayersCommandResult> commandHandler
            ) =>
            {
                var command = request.ToCommand(gameId);
                var commandResult = await commandHandler.Handle(command);

                return commandResult.ToResponse();
            }).WithName(EndpointNames.AddPlayers);

        api.MapPost(
            "/{gameId}/players/{playerId}/guess",
            async(
                Guid gameId,
                Guid playerId,
                GuessMisteryNumberRequest request,
                ICommandHandler<GuessMisteryNumberCommand, GuessMisteryNumberCommandResult> commandHandler
            ) =>
            {
                var command = request.ToCommand(gameId, playerId);
                var commandResult = await commandHandler.Handle(command);

                return commandResult.ToResponse();

            }).WithName(EndpointNames.GuessMisteryNumber);

        return app;
    }

    public static class EndpointNames
    {
        public const string CreateGame = "create_game";
        public const string AddPlayers = "add_players";
        public const string GuessMisteryNumber = "guess_mistery_number";
    }
}
