using Domain.Contracts;

namespace Domain.Model.ValueObjects;

internal sealed class MisteryNumber
{
    public int Value { get; }
    public MisteryNumberRange Range { get; }

    private MisteryNumber(MisteryNumberRange range, IRandomProvider randomNumberProvider)
    {
        Range = range;
        Value = randomNumberProvider.GetRandomNumber(range.Min, range.Max);
    }

    public static MisteryNumber Generate(MisteryNumberRange range, IRandomProvider randomNumberProvider)
    {
        return new MisteryNumber(range, randomNumberProvider);
    }
}

