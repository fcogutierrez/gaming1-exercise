using Domain.Contracts;
using Domain.Events;
using Domain.Exceptions;
using Domain.Model;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Moq;
using Shouldly;

namespace Domain.Tests.Model
{
    public sealed class GameAggregateTests
    {
        private readonly Mock<IRandomProvider> _randomProviderMock = new();
        private const int MisteryNumber = 58;

        public GameAggregateTests()
        {
            _randomProviderMock
                .Setup(m => m.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(MisteryNumber);
        }

        [Fact]
        public void Constructor_calculates_the_mistery_number()
        {
            // Arrange
            var min = 1;
            var max = 100;

            // Act
            var game = new GameAggregate(min, max, _randomProviderMock.Object);

            // Assert
            game.Changes.Count().ShouldBe(1);

            var gameCreated = game.Changes.Single().ShouldBeOfType<GameCreatedEvent>();
            gameCreated.AggregateId.ShouldNotBe(Guid.Empty);
            gameCreated.MisteryNumber.Value.ShouldBe(MisteryNumber);
            gameCreated.MisteryNumber.Range.Min.ShouldBe(min);
            gameCreated.MisteryNumber.Range.Max.ShouldBe(max);

            _randomProviderMock.Verify(m => m.GetRandomNumber(min, max), Times.Once);
        }

        [Fact]
        public void Add_players_throws_an_exception_when_player_names_are_empty()
        {
            // Arrange
            var game = new GameAggregate(1, 100, Mock.Of<IRandomProvider>());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => game.AddPlayers([]));
        }

        [Fact]
        public void Add_players_throws_an_exception_when_players_are_already_added()
        {
            // Arrange
            var game = new GameAggregate(1, 100, Mock.Of<IRandomProvider>());
            var playerNames = new[] { "Player1", "Player2" };
            game.AddPlayers(playerNames);

            // Act & Assert
            Assert.Throws<PlayersAlreadyAddedException>(() => game.AddPlayers(playerNames));
        }

        [Fact]
        public void Add_players_adds_players_and_starts_the_first_turn_in_the_first_round()
        {
            // Arrange
            var game = new GameAggregate(1, 100, Mock.Of<IRandomProvider>());
            var playerNames = new[]
            {
                "Player1",
                "Player2"
            };

            // Act
            var addPlayersResult = game.AddPlayers(playerNames);

            // Assert
            game.Changes.Count().ShouldBe(4);

            var playersAddedEvent = game.Changes.ElementAt(1).ShouldBeOfType<PlayersAddedEvent>();
            playersAddedEvent.AggregateId.ShouldBe(game.Id);

            var playerOne = playersAddedEvent.Players[0];
            var playerTwo = playersAddedEvent.Players[1];

            playerOne.Id.ShouldNotBe(Guid.Empty);
            playerOne.Name.ShouldBe("Player1");
            playerOne.Order.ShouldBe(1);
            playerOne.GuessAttempts.ShouldBeEmpty();

            playerTwo.Id.ShouldNotBe(Guid.Empty);
            playerTwo.Name.ShouldBe("Player2");
            playerTwo.Order.ShouldBe(2);
            playerTwo.GuessAttempts.ShouldBeEmpty();

            var newRoundStartedEvent = game.Changes.ElementAt(2).ShouldBeOfType<NewRoundStartedEvent>();
            newRoundStartedEvent.AggregateId.ShouldBe(game.Id);
            newRoundStartedEvent.Round.ShouldBe(1);

            var newPlayerTurnEvent = game.Changes.ElementAt(3).ShouldBeOfType<NewPlayerTurnEvent>();
            newPlayerTurnEvent.AggregateId.ShouldBe(game.Id);
            newPlayerTurnEvent.PlayerId.ShouldBe(playerOne.Id);
            newPlayerTurnEvent.Round.ShouldBe(1);

            addPlayersResult.CurrentRound.ShouldBe(1);
            addPlayersResult.PlayerTurn.Id.ShouldBe(playerOne.Id);
            addPlayersResult.Players[0].Id.ShouldBe(playerOne.Id);
            addPlayersResult.Players[0].Name.ShouldBe(playerOne.Name);
            addPlayersResult.Players[0].Order.ShouldBe(playerOne.Order);
            addPlayersResult.Players[1].Id.ShouldBe(playerTwo.Id);
            addPlayersResult.Players[1].Name.ShouldBe(playerTwo.Name);
            addPlayersResult.Players[1].Order.ShouldBe(playerTwo.Order);
        }

        [Fact]
        public void Guess_mistery_number_throws_an_exception_when_game_is_already_finished()
        {
            // Arrange
            var game = new GameAggregate(1, 100, _randomProviderMock.Object);
            var playerNames = new[] { "Player1", "Player2" };
            var addPlayersResult = game.AddPlayers(playerNames);
            game.GuessMisteryNumber(addPlayersResult.PlayerTurn.Id, MisteryNumber);

            // Act & Assert
            Assert.Throws<GameAlreadyFinishedException>(() => game.GuessMisteryNumber(addPlayersResult.PlayerTurn.Id, 50));
        }

        [Fact]
        public void Guess_mistery_number_throws_an_exception_when_player_is_not_found()
        {
            // Arrange
            var game = new GameAggregate(1, 100, Mock.Of<IRandomProvider>());
            var playerNames = new[] { "Player1", "Player2" };
            game.AddPlayers(playerNames);

            // Act & Assert
            Assert.Throws<PlayerNotFoundException>(() => game.GuessMisteryNumber(Guid.NewGuid(), 50));
        }

        [Fact]
        public void Guess_mistery_number_throws_an_exception_when_it_is_not_player_turn()
        {
            // Arrange
            var game = new GameAggregate(1, 100, Mock.Of<IRandomProvider>());
            var playerNames = new[] { "Player1", "Player2" };
            game.AddPlayers(playerNames);

            var playersAddedEvent = game.Changes.ElementAt(1).ShouldBeOfType<PlayersAddedEvent>();
            var secondPlayerId = playersAddedEvent.Players[1].Id;

            // Act & Assert
            Assert.Throws<WrongPlayerTurnException>(() => game.GuessMisteryNumber(secondPlayerId, 50));
        }

        [Fact]
        public void Guess_mistery_number_returns_next_turn_result_when_guess_from_first_player_is_too_low()
        {
            // Arrange
            var game = new GameAggregate(1, 100, _randomProviderMock.Object);
            var playerNames = new[] { "Player1", "Player2" };
            var addPlayersResult = game.AddPlayers(playerNames);            

            // Act
            var result = game.GuessMisteryNumber(addPlayersResult.PlayerTurn.Id, 14);

            // Assert
            var playerOneId = GetPlayerId(game, index: 0);
            var playerTwoId = GetPlayerId(game, index: 1);

            result.TryPickT0(out var nextTurnResult, out _).ShouldBeTrue();
            nextTurnResult.CurrentRound.ShouldBe(1);
            nextTurnResult.PlayerTurn.Id.ShouldBe(playerTwoId);
            nextTurnResult.PreviousPlayerGuessStatus.ShouldBe(FailedGuessHint.TooLow);

            game.Changes.Count().ShouldBe(6);

            var guessTooLowEvent = game.Changes.ElementAt(4).ShouldBeOfType<GuessTooLowEvent>();
            guessTooLowEvent.AggregateId.ShouldBe(game.Id);
            guessTooLowEvent.PlayerId.ShouldBe(playerOneId);
            guessTooLowEvent.GuessAttempt.Guess.ShouldBe(14);
            guessTooLowEvent.GuessAttempt.Status.ShouldBe(PlayerGuessStatus.TooLow);

            var newPlayerTurnEvent = game.Changes.ElementAt(5).ShouldBeOfType<NewPlayerTurnEvent>();
            newPlayerTurnEvent.AggregateId.ShouldBe(game.Id);
            newPlayerTurnEvent.PlayerId.ShouldBe(playerTwoId);
            newPlayerTurnEvent.Round.ShouldBe(1);
        }

        [Fact]
        public void Guess_mistery_number_returns_next_turn_result_when_guess_from_first_player_is_too_high()
        {
            // Arrange
            var game = new GameAggregate(1, 100, _randomProviderMock.Object);
            var playerNames = new[] { "Player1", "Player2" };
            var addPlayersResult = game.AddPlayers(playerNames);

            // Act
            var result = game.GuessMisteryNumber(addPlayersResult.PlayerTurn.Id, 83);

            // Assert
            var playerOneId = GetPlayerId(game, 0);
            var playerTwoId = GetPlayerId(game, 1);

            result.TryPickT0(out var nextTurnResult, out _).ShouldBeTrue();
            nextTurnResult.CurrentRound.ShouldBe(1);
            nextTurnResult.PlayerTurn.Id.ShouldBe(playerTwoId);
            nextTurnResult.PreviousPlayerGuessStatus.ShouldBe(FailedGuessHint.TooHigh);

            game.Changes.Count().ShouldBe(6);

            var guessTooHighEvent = game.Changes.ElementAt(4).ShouldBeOfType<GuessTooHighEvent>();
            guessTooHighEvent.AggregateId.ShouldBe(game.Id);
            guessTooHighEvent.PlayerId.ShouldBe(playerOneId);
            guessTooHighEvent.GuessAttempt.Guess.ShouldBe(83);
            guessTooHighEvent.GuessAttempt.Status.ShouldBe(PlayerGuessStatus.TooHigh);

            var newPlayerTurnEvent = game.Changes.ElementAt(5).ShouldBeOfType<NewPlayerTurnEvent>();
            newPlayerTurnEvent.AggregateId.ShouldBe(game.Id);
            newPlayerTurnEvent.PlayerId.ShouldBe(playerTwoId);
            newPlayerTurnEvent.Round.ShouldBe(1);
        }

        [Fact]
        public void Guess_mistery_number_returns_next_turn_result_and_starts_second_round_when_guess_from_second_player_has_failed()
        {
            // Arrange
            var game = new GameAggregate(1, 100, _randomProviderMock.Object);
            var playerNames = new[] { "Player1", "Player2" };
            var addPlayersResult = game.AddPlayers(playerNames);
            var firstGuessResult = game.GuessMisteryNumber(addPlayersResult.PlayerTurn.Id, 83);
            firstGuessResult.TryPickT0(out NextTurnResult secondTurn, out _);

            // Act
            var secondGuessResult = game.GuessMisteryNumber(secondTurn.PlayerTurn.Id, 23);

            // Assert
            var playerOneId = GetPlayerId(game, index: 0);
            var playerTwoId = GetPlayerId(game, index: 1);

            secondGuessResult.TryPickT0(out var thirdTurn, out _).ShouldBeTrue();
            thirdTurn.CurrentRound.ShouldBe(2);
            thirdTurn.PlayerTurn.Id.ShouldBe(playerOneId);
            thirdTurn.PreviousPlayerGuessStatus.ShouldBe(FailedGuessHint.TooLow);

            game.Changes.Count().ShouldBe(9);

            var newRoundStartedEvent = game.Changes.ElementAt(7).ShouldBeOfType<NewRoundStartedEvent>();
            newRoundStartedEvent.AggregateId.ShouldBe(game.Id);
            newRoundStartedEvent.Round.ShouldBe(2);

            var newPlayerTurnEvent = game.Changes.ElementAt(8).ShouldBeOfType<NewPlayerTurnEvent>();
            newPlayerTurnEvent.AggregateId.ShouldBe(game.Id);
            newPlayerTurnEvent.PlayerId.ShouldBe(playerOneId);
            newPlayerTurnEvent.Round.ShouldBe(2);
        }

        [Fact]
        public void Guess_mistery_number_returns_number_guessed_result()
        {
            // Arrange
            var game = new GameAggregate(1, 100, _randomProviderMock.Object);
            var playerNames = new[] { "Player1", "Player2" };
            var addPlayersResult = game.AddPlayers(playerNames);
            
            var firstGuessResult = game.GuessMisteryNumber(addPlayersResult.PlayerTurn.Id, 83);
            firstGuessResult.TryPickT0(out NextTurnResult secondTurn, out _);

            var secondGuessResult = game.GuessMisteryNumber(secondTurn.PlayerTurn.Id, 23);
            secondGuessResult.TryPickT0(out var thirdTurn, out _).ShouldBeTrue();

            var thirdGuessResult = game.GuessMisteryNumber(thirdTurn.PlayerTurn.Id, 45);
            firstGuessResult.TryPickT0(out NextTurnResult fourthTurn, out _);

            // Act
            var fourthGuessResult = game.GuessMisteryNumber(fourthTurn.PlayerTurn.Id, MisteryNumber);

            // Assert
            var playerOneId = GetPlayerId(game, index: 0);
            var playerTwoId = GetPlayerId(game, index: 1);

            fourthGuessResult.TryPickT1(out MisteryNumberGuessedResult numberGuessed, out _);

            numberGuessed.Value.ShouldBe(MisteryNumber);
            numberGuessed.Winner.Id.ShouldBe(playerTwoId);

            var guessCorrectEvent = game.Changes.ElementAt(11).ShouldBeOfType<GuessCorrectEvent>();
            guessCorrectEvent.AggregateId.ShouldBe(game.Id);
            guessCorrectEvent.PlayerId.ShouldBe(playerTwoId);
            guessCorrectEvent.GuessAttempt.Guess.ShouldBe(MisteryNumber);
            guessCorrectEvent.GuessAttempt.Status.ShouldBe(PlayerGuessStatus.Correct);
        }

        private static Guid GetPlayerId(GameAggregate game, int index)
        {
            var playersAddedEvent = game.Changes.ElementAt(1).ShouldBeOfType<PlayersAddedEvent>();
            var player = playersAddedEvent.Players[index];

            return player.Id;
        }
    }
}
