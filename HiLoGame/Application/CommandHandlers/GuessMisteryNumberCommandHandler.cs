using Application.Commands;
using Application.Contracts;
using Domain.Factories;
using Domain.Model;
using OneOf;

namespace Application.CommandHandlers;

using GuessMisteryNumberCommandResult = OneOf<NextTurnCommandResult, MisteryNumberGuessedCommandResult>;

public sealed class GuessMisteryNumberCommandHandler(IEventStorage eventStorage)
    : ICommandHandler<GuessMisteryNumberCommand, GuessMisteryNumberCommandResult>
{
    private readonly IEventStorage _eventStorage = eventStorage;

    public async Task<GuessMisteryNumberCommandResult> Handle(GuessMisteryNumberCommand command)
    {
        var events = await _eventStorage.GetByAggregateId(command.GameId);
        var game = AggregateFactory.Create<GameAggregate>(events);

        var result = game.GuessMisteryNumber(command.PlayerId, command.Guess);
        await _eventStorage.SaveMany(game.Changes);

        return result.Match<GuessMisteryNumberCommandResult>(
            nextTurn => new NextTurnCommandResult(nextTurn.PlayerTurn, nextTurn.CurrentRound, nextTurn.PreviousPlayerGuessStatus),
            numberGuessed => new MisteryNumberGuessedCommandResult(numberGuessed.Winner, numberGuessed.Value)
        );
    }
}
