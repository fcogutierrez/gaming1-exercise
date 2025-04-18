namespace HiLoGame.Api;

internal static class Endpoints
{
    public static IEndpointRouteBuilder UseHiLoGameEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/games/hilo");

        api.MapGet("/", () => Results.Ok("Welcome to the HiLo Game API!"));

        return app;
    }
}
