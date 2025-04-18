using Domain.Contracts;
using Domain.Events;
using Domain.Model;
using Domain.Model.Entities;
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
        public void Add_players_adds_players_and_starts_the_round()
        {
            // Arrange
            var game = new GameAggregate(1, 100, Mock.Of<IRandomProvider>());
            var playerNames = new[]
            {
                "Player1",
                "Player2"
            };

            // Act
            var result = game.AddPlayers(playerNames);

            // Assert
            game.Changes.Count().ShouldBe(4);

            var playersAddedEvent = game.Changes.ElementAt(1).ShouldBeOfType<PlayersAddedEvent>();
            playersAddedEvent.AggregateId.ShouldBe(game.Id);

            var firstPlayer = playersAddedEvent.Players[0];
            var secondPlayer = playersAddedEvent.Players[1];

            firstPlayer.Id.ShouldNotBe(Guid.Empty);
            firstPlayer.Name.ShouldBe("Player1");
            firstPlayer.Order.ShouldBe(1);
            firstPlayer.GuessAttempts.ShouldBeEmpty();

            secondPlayer.Id.ShouldNotBe(Guid.Empty);
            secondPlayer.Name.ShouldBe("Player2");
            secondPlayer.Order.ShouldBe(2);
            secondPlayer.GuessAttempts.ShouldBeEmpty();

            var newRoundStartedEvent = game.Changes.ElementAt(2).ShouldBeOfType<NewRoundStartedEvent>();
            newRoundStartedEvent.AggregateId.ShouldBe(game.Id);
            newRoundStartedEvent.Round.ShouldBe(1);

            var newPlayerTurnEvent = game.Changes.ElementAt(3).ShouldBeOfType<NewPlayerTurnEvent>();
            newPlayerTurnEvent.AggregateId.ShouldBe(game.Id);
            newPlayerTurnEvent.PlayerId.ShouldBe(firstPlayer.Id);
            newPlayerTurnEvent.Round.ShouldBe(1);
        }
    }
}
