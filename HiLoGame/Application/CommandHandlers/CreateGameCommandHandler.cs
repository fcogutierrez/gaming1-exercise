using Application.Commands;
using Application.Contracts;
using Domain.Contracts;
using Domain.Model;

namespace Application.CommandHandlers;

public sealed class CreateGameCommandHandler (IEventStorage eventStorage, IRandomProvider randomProvider)
    : ICommandHandler<CreateGameCommand, CreateGameCommandResult>
{
    private readonly IEventStorage _eventStorage = eventStorage;
    private readonly IRandomProvider _randomProvider = randomProvider;

    public async Task<CreateGameCommandResult> Handle(CreateGameCommand command)
    {
        var game = new GameAggregate(command.Min, command.Max, _randomProvider);
        await _eventStorage.SaveMany(game.Changes);

        return new CreateGameCommandResult();
    }
}
