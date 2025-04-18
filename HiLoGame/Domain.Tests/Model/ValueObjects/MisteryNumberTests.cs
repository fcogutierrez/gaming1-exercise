using Domain.Contracts;
using Domain.Model.ValueObjects;
using Moq;
using Shouldly;

namespace Domain.Tests.Model.ValueObjects;

public sealed class MisteryNumberTests
{
    [Fact]
    public void Generate_creates_a_mistery_number_within_the_range()
    {
        // Arrange
        var min = 1;
        var max = 10;
        var value = 5;

        var randomProviderMock = new Mock<IRandomProvider>();
        randomProviderMock
            .Setup(m => m.GetRandomNumber(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(5);

        var range = MisteryNumberRange.Create(min, max);

        // Act
        var misteryNumber = MisteryNumber.Generate(range, randomProviderMock.Object);

        // Assert
        misteryNumber.Value.ShouldBe(value);
        randomProviderMock.Verify(x => x.GetRandomNumber(min, max), Times.Once);
    }
}
