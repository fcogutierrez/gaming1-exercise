namespace Domain.Model.ValueObjects;

internal sealed class MisteryNumber
{
    public int Value { get; }
    public MisteryNumberRange Range { get; }

    private MisteryNumber(MisteryNumberRange range)
    {
        Range = range;
        var random = new Random();
        Value = random.Next(range.Min, range.Max +1);
    }

    public static MisteryNumber Generate(MisteryNumberRange range)
    {
        return new MisteryNumber(range);
    }
}

