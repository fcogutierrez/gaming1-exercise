using Domain.Contracts;
using Domain.Events;
using Domain.Model.Base;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.ValueObjects;

namespace Domain.Model;

public sealed class GameAggregate : AggregateBase
{
    private int _round = 1;
    private MisteryNumber _misteryNumber = null!;
    private List<Player> _players = [];
    private int _maxOrder = 0;
    private Player _nextPlayer = null!;
    private Guid? _winnerId = null;

    public GameAggregate(int min, int max, IRandomProvider randomProvider) : base()
    {        
        var range = MisteryNumberRange.Create(min, max);
        var misteryNumber = MisteryNumber.Generate(range, randomProvider);

        var @event = new GameCreatedEvent(Guid.NewGuid(), misteryNumber);
        ApplyDomainEvent(@event);
    }

    public CreatePlayersResult CreatePlayers(IEnumerable<string> playerNames)
    {
        if (!playerNames.Any())
        {
            throw new ArgumentException("Player names cannot be empty", nameof(playerNames));
        }

        var players = playerNames.Select((name, index) => Player.Create(Guid.NewGuid(), index + 1, name)).ToList();
        ApplyDomainEvent(new PlayersCreatedEvent(Id, players));

        ApplyDomainEvent(new NewRoundStartedEvent(Id, _round));

        var nextPlayer = GetNewNextPlayer();
        ApplyDomainEvent(new NewPlayerTurnEvent(Id, nextPlayer.Id, _round));

        return new CreatePlayersResult(nextPlayer.Id, nextPlayer.Order, nextPlayer.Name, _round);
    }

    public void GuessMisteryNumber(Guid playerId, int guess)
    {
        if (_winnerId is not null)
        {
            throw new InvalidOperationException("Game is already finished");
        }

        var player = _players.SingleOrDefault(p => p.Id == playerId);
        if (player is null)
        {
            throw new ArgumentException($"Player with ID {playerId} was not found", nameof(playerId));
        }

        if (_nextPlayer.Id != player.Id)
        {
            throw new InvalidOperationException($"It's not {player.Name}'s turn");
        }

        var guessAttempt = GuessAttempt.Create(guess, _misteryNumber);

        switch (guessAttempt.Status)
        {
            case PlayerGuessStatus.TooLow:
                ApplyDomainEvent(new GuessTooLowEvent(Id, playerId, guessAttempt));
                break;
            case PlayerGuessStatus.TooHigh:
                ApplyDomainEvent(new GuessTooHighEvent(Id, playerId, guessAttempt));
                break;
            case PlayerGuessStatus.Correct:
                ApplyDomainEvent(new GuessCorrectEvent(Id, playerId, guessAttempt));
                break;
            default:
                throw new InvalidOperationException("Invalid guess attempt status");
        }

        if (IsNextPlayerTheLastInRound())
        {
            int newRound = _round + 1;
            ApplyDomainEvent(new NewRoundStartedEvent(Id, newRound));
        }

        var newNextPlayer = GetNewNextPlayer();
        ApplyDomainEvent(new NewPlayerTurnEvent(Id, newNextPlayer.Id, _round));
    }

    protected override void RegisterDomainEventAppliers()
    {
        RegisterDomainEventApplier<GameCreatedEvent>(Apply);
        RegisterDomainEventApplier<PlayersCreatedEvent>(Apply);
        RegisterDomainEventApplier<NewRoundStartedEvent>(Apply);
        RegisterDomainEventApplier<NewPlayerTurnEvent>(Apply);
        RegisterDomainEventApplier<GuessTooHighEvent>(Apply);
        RegisterDomainEventApplier<GuessTooLowEvent>(Apply);
        RegisterDomainEventApplier<GuessCorrectEvent>(Apply);
    }

    private bool IsNextPlayerTheLastInRound() => _nextPlayer.Order == _maxOrder;

    private Player GetNewNextPlayer()
    {
        if (_nextPlayer is null)
        {
            throw new InvalidOperationException("Next player index is not set");
        }

        var nextPlayerIndex = (_nextPlayer.Order + 1) % _players.Count;
        return _players.ElementAt(nextPlayerIndex);
    }

    private void Apply(GameCreatedEvent @event)
    {
        Id = @event.AggregateId;
        _misteryNumber = @event.MisteryNumber;

        Save(@event);
    }

    private void Apply(PlayersCreatedEvent @event)
    {
        _players = @event.Players;
        _maxOrder = _players.Max(p => p.Order);

        Save(@event);
    }

    private void Apply(NewRoundStartedEvent @event)
    {
        _round = @event.Round;

        Save(@event);
    }

    private void Apply(NewPlayerTurnEvent @event)
    {
        _nextPlayer = _players.Single(p => p.Id == @event.PlayerId);

        Save(@event);
    }

    private void Apply(GuessTooHighEvent @event)
    {
        var player = _players.Single(p => p.Id == @event.PlayerId);
        player.AddGuessAttempt(@event.GuessAttempt);

        Save(@event);
    }

    private void Apply(GuessTooLowEvent @event)
    {
        var player = _players.Single(p => p.Id == @event.PlayerId);
        player.AddGuessAttempt(@event.GuessAttempt);

        Save(@event);
    }

    private void Apply(GuessCorrectEvent @event)
    {
        _winnerId = @event.PlayerId;
        var player = _players.Single(p => p.Id == @event.PlayerId);
        player.AddGuessAttempt(@event.GuessAttempt);

        Save(@event);
    }
}

public sealed record CreatePlayersResult(Guid NextPlayerId, int NextPlayerOrder, string NextPlayerNam, int CurrentRound);