namespace Domain.Exceptions;

public sealed class WrongPlayerTurnException(Guid gameId, Guid playerId) 
    : Exception($"It is not the turn of the player {playerId} in the game {gameId}");
