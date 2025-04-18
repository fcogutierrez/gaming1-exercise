using Domain.Model.ValueObjects;

namespace Domain.Model.Entities;

internal sealed class Player
{
    public int Id { get; }
    public int Order { get; set; }
    public string Name { get; }
    public IList<GuessAttempt> GuessAttempts { get; } = [];

    private Player(int id, int order, string name)
    {
        Id = id;
        Name = name;
        Order = order;
    }

    public static Player Create(int id, int order, string name)
    {
        if (id < 0)
        {
            throw new ArgumentException("Player ID must be non-negative", nameof(id));
        }
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
