namespace Domain.Exceptions;

public sealed class PlayersAlreadyAddedException(Guid gameId) 
    : Exception($"Players have been already added in game {gameId}");
