using Domain.Contracts;
using Domain.Model.Enums;
using Domain.Model.ValueObjects;
using Moq;
using Shouldly;

namespace Domain.Tests.Model;

public sealed class GuessAttemptTests
{
    [Fact]
    public void Create_throws_an_exception_when_guess_is_negative()
    {
        // Arrange
        var guess = -1;

        var randomNumberProvider = Mock.Of<IRandomProvider>();
        var misteryNumber = MisteryNumber.Generate(MisteryNumberRange.Create(0, 10), randomNumberProvider);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => GuessAttempt.Create(guess, misteryNumber));
    }

    [Fact]
    public void Create_returns_correct_guess_attempt_when_guess_is_equal_to_mistery_number()
    {
        // Arrange
        var guess = 5;

        var randomProviderMock = new Mock<IRandomProvider>();
        randomProviderMock
            .Setup(m => m.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(guess);

        var misteryNumber = MisteryNumber.Generate(MisteryNumberRange.Create(0, 10), randomProviderMock.Object);

        // Act
        var guessAttempt = GuessAttempt.Create(guess, misteryNumber);

        // Assert
        guessAttempt.Guess.ShouldBe(guess);
        guessAttempt.Status.ShouldBe(PlayerGuessStatus.Correct);
    }

    [Fact]
    public void Create_returns_too_low_guess_attempt_when_guess_is_less_than_mistery_number()
    {
        // Arrange
        var guess = 1;

        var randomProviderMock = new Mock<IRandomProvider>();
        randomProviderMock
            .Setup(m => m.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(9);

        var misteryNumber = MisteryNumber.Generate(MisteryNumberRange.Create(1, 10), randomProviderMock.Object);

        // Act
        var guessAttempt = GuessAttempt.Create(guess, misteryNumber);

        // Assert
        guessAttempt.Guess.ShouldBe(guess);
        guessAttempt.Status.ShouldBe(PlayerGuessStatus.TooLow);
    }

    [Fact]
    public void Create_returns_too_high_guess_attempt_when_guess_is_greater_than_mistery_number()
    {
        // Arrange
        var guess = 9;
        var randomProviderMock = new Mock<IRandomProvider>();
        randomProviderMock
            .Setup(m => m.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(1);
        var misteryNumber = MisteryNumber.Generate(MisteryNumberRange.Create(1, 10), randomProviderMock.Object);

        // Act
        var guessAttempt = GuessAttempt.Create(guess, misteryNumber);

        // Assert
        guessAttempt.Guess.ShouldBe(guess);
        guessAttempt.Status.ShouldBe(PlayerGuessStatus.TooHigh);
    }
}
