using Application.Commands;
using Application.Contracts;
using Domain.Factories;
using Domain.Model;

namespace Application.CommandHandlers;

public sealed class GuessMisteryNumberCommandHandler(IEventStorage eventStorage)
    : ICommandHandler<GuessMisteryNumberCommand, GuessMisteryNumberCommandResult>
{
    private readonly IEventStorage _eventStorage = eventStorage;

    public async Task<GuessMisteryNumberCommandResult> Handle(GuessMisteryNumberCommand command)
    {
        var events = await _eventStorage.GetByAggregateId(command.GameId);
        var game = AggregateFactory.Create<GameAggregate>(events);

        game.GuessMisteryNumber(command.PlayerId, command.Guess);

        return new GuessMisteryNumberCommandResult();
    }
}
