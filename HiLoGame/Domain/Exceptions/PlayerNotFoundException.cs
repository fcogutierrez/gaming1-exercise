namespace Domain.Exceptions;

public sealed class PlayerNotFoundException(Guid gameId, Guid playerId) 
    : Exception($"The player {playerId} was not found in the game {gameId}");
