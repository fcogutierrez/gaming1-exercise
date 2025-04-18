namespace HiLoGame.Api.Requests;

public sealed record CreateGameRequest(int Min, int Max);

public sealed record CreateGameResponse(Guid Id);
