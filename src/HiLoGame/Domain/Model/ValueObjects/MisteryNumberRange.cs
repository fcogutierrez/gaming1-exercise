namespace Domain.Model.ValueObjects;

internal sealed record MisteryNumberRange
{
    public int Min { get; }
    public int Max { get; }

    private MisteryNumberRange(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public static MisteryNumberRange Create(int min, int max)
    {
        if (min < 0 || max < 0)
        {
            throw new ArgumentException("Min and Max values must be non-negative.");
        }
        if (min >= max)
        {
            throw new ArgumentException("Min value must be less than Max value.");
        }

        return new MisteryNumberRange(min, max);
    }
}

