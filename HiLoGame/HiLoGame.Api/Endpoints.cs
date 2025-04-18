using Application.Commands;
using Application.Contracts;
using HiLoGame.Api.Requests;

namespace HiLoGame.Api;

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
                var command = CreateGameRequest.ToCommand(request);
                var result = await commandHandler.Handle(command);

                return result.ToResponse();
            }).WithName(EndpointNames.CreateGame);

        return app;
    }

    public static class EndpointNames
    {
        public const string CreateGame = "create_game";
    }
}
