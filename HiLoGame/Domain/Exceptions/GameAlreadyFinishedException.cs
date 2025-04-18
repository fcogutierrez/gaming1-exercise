namespace Domain.Exceptions;

public sealed class GameAlreadyFinishedException(Guid gameId) 
    : Exception($"The game {gameId} has been already finished");
