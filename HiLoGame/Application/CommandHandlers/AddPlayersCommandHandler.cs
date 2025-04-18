using Application.Commands;
using Application.Contracts;
using Domain.Factories;
using Domain.Model;

namespace Application.CommandHandlers;

public sealed class AddPlayersCommandHandler(IEventStorage eventStorage) 
    : ICommandHandler<AddPlayersCommand, AddPlayersCommandResult>
{
    private readonly IEventStorage _eventStorage = eventStorage;

    public async Task<AddPlayersCommandResult> Handle(AddPlayersCommand command)
    {
        var events = await _eventStorage.GetByAggregateId(command.GameId);
        var game = AggregateFactory.Create<GameAggregate>(events);

        var result  = game.AddPlayers(command.PlayerNames);
        await _eventStorage.SaveMany(game.Changes);

        return new AddPlayersCommandResult(result.PlayerTurn, result.CurrentRound);
    }
}