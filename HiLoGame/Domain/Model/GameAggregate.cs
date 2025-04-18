using Domain.Contracts;
using Domain.Events;
using Domain.Model.Base;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.ValueObjects;
using OneOf;

namespace Domain.Model;

using GuessMisteryNumberResult = OneOf<NextTurnResult, MisteryNumberGuessedResult>;

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

    public GameAggregate(IList<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            ApplyDomainEvent(domainEvent);
        }
    }

    public InitialTurnResult AddPlayers(IEnumerable<string> playerNames)
    {
        if (!playerNames.Any())
        {
            throw new ArgumentException("Player names cannot be empty", nameof(playerNames));
        }

        var players = playerNames.Select((name, index) => Player.Create(Guid.NewGuid(), index + 1, name)).ToList();
        ApplyDomainEvent(new PlayersAddedEvent(Id, players));

        ApplyDomainEvent(new NewRoundStartedEvent(Id, _round));

        var nextPlayer = GetNewNextPlayer();
        ApplyDomainEvent(new NewPlayerTurnEvent(Id, nextPlayer.Id, _round));

        return new InitialTurnResult(new PlayerTurn(nextPlayer.Id, nextPlayer.Order, nextPlayer.Name), _round);
    }

    public GuessMisteryNumberResult GuessMisteryNumber(Guid playerId, int guess)
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

        if (guessAttempt.Status is PlayerGuessStatus.TooLow)
        {
            ApplyDomainEvent(new GuessTooLowEvent(Id, playerId, guessAttempt));
        }
        else if (guessAttempt.Status is PlayerGuessStatus.TooHigh)
        {
            ApplyDomainEvent(new GuessTooHighEvent(Id, playerId, guessAttempt));
        }
        else if (guessAttempt.Status is PlayerGuessStatus.Correct)
        {
            ApplyDomainEvent(new GuessCorrectEvent(Id, playerId, guessAttempt));
            return new MisteryNumberGuessedResult(new WinnerPlayer(player.Id, player.Order, player.Name), guess);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(guessAttempt.Status), guessAttempt.Status, "The guess status is not supporteda");
        }

        if (IsCurrentPlayerTheLastInRound())
        {
            int newRound = _round + 1;
            ApplyDomainEvent(new NewRoundStartedEvent(Id, newRound));
        }

        var newNextPlayer = GetNewNextPlayer();
        ApplyDomainEvent(new NewPlayerTurnEvent(Id, newNextPlayer.Id, _round));

        return new NextTurnResult(new PlayerTurn(newNextPlayer.Id, newNextPlayer.Order, newNextPlayer.Name), _round, MapToFailedGuessHint(guessAttempt.Status));
    }

    protected override void RegisterDomainEventAppliers()
    {
        RegisterDomainEventApplier<GameCreatedEvent>(Apply);
        RegisterDomainEventApplier<PlayersAddedEvent>(Apply);
        RegisterDomainEventApplier<NewRoundStartedEvent>(Apply);
        RegisterDomainEventApplier<NewPlayerTurnEvent>(Apply);
        RegisterDomainEventApplier<GuessTooHighEvent>(Apply);
        RegisterDomainEventApplier<GuessTooLowEvent>(Apply);
        RegisterDomainEventApplier<GuessCorrectEvent>(Apply);
    }

    private bool IsCurrentPlayerTheLastInRound() => _nextPlayer.Order == _maxOrder;

    private Player GetNewNextPlayer()
    {
        if (_nextPlayer is null)
        {
            return _players.First();
        }

        var nextPlayerIndex = _nextPlayer.Order % _players.Count;
        return _players.ElementAt(nextPlayerIndex);
    }

    private FailedGuessHint MapToFailedGuessHint(PlayerGuessStatus result)
    {
        return result switch
        {
            PlayerGuessStatus.TooHigh => FailedGuessHint.TooHigh,
            PlayerGuessStatus.TooLow => FailedGuessHint.TooLow,
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };
    }

    private void Apply(GameCreatedEvent @event)
    {
        Id = @event.AggregateId;
        _misteryNumber = @event.MisteryNumber;
    }

    private void Apply(PlayersAddedEvent @event)
    {
        _players = @event.Players;
        _maxOrder = _players.Max(p => p.Order);
    }

    private void Apply(NewRoundStartedEvent @event)
    {
        _round = @event.Round;
    }

    private void Apply(NewPlayerTurnEvent @event)
    {
        _nextPlayer = _players.Single(p => p.Id == @event.PlayerId);
    }

    private void Apply(GuessTooHighEvent @event)
    {
        var player = _players.Single(p => p.Id == @event.PlayerId);
        player.AddGuessAttempt(@event.GuessAttempt);
    }

    private void Apply(GuessTooLowEvent @event)
    {
        var player = _players.Single(p => p.Id == @event.PlayerId);
        player.AddGuessAttempt(@event.GuessAttempt);
    }

    private void Apply(GuessCorrectEvent @event)
    {
        _winnerId = @event.PlayerId;
        var player = _players.Single(p => p.Id == @event.PlayerId);
        player.AddGuessAttempt(@event.GuessAttempt);
    }
}

public sealed record InitialTurnResult(PlayerTurn PlayerTurn, int CurrentRound);

public sealed record NextTurnResult(PlayerTurn PlayerTurn, int CurrentRound, FailedGuessHint PreviousPlayerGuessStatus);

public sealed record MisteryNumberGuessedResult(WinnerPlayer Winner, int Value);

public record PlayerTurn(Guid Id, int Order, string Name);

public sealed record WinnerPlayer(Guid Id, int Order, string Name)
    : PlayerTurn(Id, Order, Name);

public enum FailedGuessHint
{
    TooHigh,
    TooLow
}