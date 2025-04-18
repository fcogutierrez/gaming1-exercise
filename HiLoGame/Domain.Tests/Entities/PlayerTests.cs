using Domain.Contracts;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Model.ValueObjects;
using Moq;
using Shouldly;

namespace Domain.Tests.Entities;

public sealed class PlayerTests
{
    [Fact]
    public void Create_throws_an_exception_when_order_is_negative()
    {
        // Arrange
        var id = Guid.NewGuid();
        var order = -1;
        var name = "Player1";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Player.Create(id, order, name));
    }

    [Fact]
    public void Create_throws_an_exception_when_name_is_null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var order = 1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Player.Create(id, order, null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Create_throws_an_exception_when_name_is_empty(string name)
    {
        // Arrange
        var id = Guid.NewGuid();
        var order = 1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Player.Create(id, order, name));
    }

    [Fact]
    public void Create_returns_correct_player()
    {
        // Arrange
        var id = Guid.NewGuid();
        var order = 1;
        var name = "Player1";

        // Act
        var player = Player.Create(id, order, name);

        // Assert
        player.Id.ShouldBe(id);
        player.Order.ShouldBe(order);
        player.Name.ShouldBe(name);
    }

    [Fact]
    public void AddGuessAttempt_adds_guess_attempt_to_player()
    {
        // Arrange
        var id = Guid.NewGuid();
        var order = 1;
        var name = "Player1";
        var player = Player.Create(id, order, name);
        var randomNumberProvider = Mock.Of<IRandomProvider>();
        var misteryNumber = MisteryNumber.Generate(MisteryNumberRange.Create(0, 10), randomNumberProvider);
        var guessAttempt = GuessAttempt.Create(5, misteryNumber);

        // Act
        player.AddGuessAttempt(guessAttempt);

        // Assert
        player.GuessAttempts.ShouldContain(guessAttempt);
        player.GuessAttempts.Count.ShouldBe(1);
    }
}
