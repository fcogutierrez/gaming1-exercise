using Domain.Events;
using Domain.Model.Base;
using Domain.Model.Entities;
using Domain.Model.ValueObjects;

namespace Domain.Model;

internal sealed class GameAggregate : AggregateBase
{
    private MisteryNumber _misteryNumber = null!;
    private IEnumerable<Player> _players = [];

    public GameAggregate(int min, int max, IEnumerable<string> playerNames) : base()
    {
        if (!playerNames.Any())
        {
            throw new ArgumentException("Player names cannot be empty", nameof(playerNames));
        }

        var range = MisteryNumberRange.Create(min, max);
        var misteryNumber = MisteryNumber.Generate(range);
        var players = playerNames.Select((name, index) => new Player(index, name));

        var @event = new GameCreatedEvent(Guid.NewGuid(), misteryNumber, players);
        Apply(@event);
    }

    public void GuessMisteryNumber(int playerId, int guess)
    {
        var player = _players.SingleOrDefault(p => p.Id == playerId);
        if (player is null)
        {
            throw new ArgumentException($"Player with ID {playerId} not found", nameof(playerId));
        }

        if (guess < _misteryNumber.Value)
        {
            var @event = new GuessTooLowEvent(Id, playerId, guess);
            Apply(@event);
        }
        else if (guess > _misteryNumber.Value)
        {
            var @event = new GuessTooHighEvent(Id, playerId, guess);
            Apply(@event);
        }
        else
        {
            var @event = new GuessCorrectEvent(Id, playerId, guess);
            Apply(@event);
        }
    }

    private void Apply(GameCreatedEvent @event)
    {
        Id = @event.Id;
        _misteryNumber = @event.MisteryNumber;
        _players = @event.Players;

        Save(@event);
    }

    private void Apply(GuessTooHighEvent @event) { }

    private void Apply(GuessTooLowEvent @event) { }

    private void Apply(GuessCorrectEvent @event) { }
}
