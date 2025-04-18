using Domain.Model.ValueObjects;

namespace Domain.Model.Entities;

internal sealed class Player
{
    public Guid Id { get; }
    public int Order { get; set; }
    public string Name { get; }
    public IList<GuessAttempt> GuessAttempts { get; } = [];

    private Player(Guid id, int order, string name)
    {
        Id = id;
        Name = name;
        Order = order;
    }

    public static Player Create(Guid id, int order, string name)
    {
        if (order < 0)
        {
            throw new ArgumentException("Player order must be non-negative", nameof(order));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Player name cannot be empty", nameof(name));
        }

        return new Player(id, order, name);
    }

    public void AddGuessAttempt(GuessAttempt guessAttempt)
    {
        GuessAttempts.Add(guessAttempt);
    }
}
