using Domain.Model.ValueObjects;

namespace Domain.Tests.Model.ValueObjects;

public sealed class MisteryNumberRangeTests
{
    [Fact]
    public void Create_throws_an_exception_when_min_is_negative()
    {
        // Arrange
        int min = -1;
        int max = 10;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => MisteryNumberRange.Create(min, max));
    }

    [Fact]
    public void Create_throws_an_exception_when_max_is_negative()
    {
        // Arrange
        int min = 1;
        int max = -10;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => MisteryNumberRange.Create(min, max));
    }

    [Fact]
    public void Create_throws_an_exception_when_min_is_equal_than_max()
    {
        // Arrange
        int min = 5;
        int max = 5;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => MisteryNumberRange.Create(min, max));
    }

    [Fact]
    public void Create_throws_an_exception_when_min_is_greater_than_max()
    {
        // Arrange
        int min = 10;
        int max = 1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => MisteryNumberRange.Create(min, max));
    }
}
