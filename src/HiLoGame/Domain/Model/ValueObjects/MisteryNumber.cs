namespace Domain.Model.ValueObjects;

internal sealed record MisteryNumber
{
    public int Value { get; }

    private MisteryNumber(MisteryNumberRange range)
    {
        var random = new Random();
        Value = random.Next(range.Min, range.Max +1);
    }

    public static MisteryNumber Generate(MisteryNumberRange range)
    {
        return new MisteryNumber(range);
    }
}

