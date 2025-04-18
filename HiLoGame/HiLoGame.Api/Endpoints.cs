using HiLoGame.Api.Requests;

namespace HiLoGame.Api;

internal static class Endpoints
{
    public static IEndpointRouteBuilder UseHiLoGameEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/games/hilo");

        api.MapPost(
            "/", (CreateGameRequest request) =>
            {
                return Results.Ok("Welcome to the HiLo Game API!");
            }).WithName(EndpointNames.CreateGame);

        return app;
    }

    public static class EndpointNames
    {
        public const string CreateGame = "create_game";
    }
}
