namespace Domain.Model.Entities;


internal sealed class Player
{
    public int Id { get; }
    public string Name { get; }

    private Player(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Player Create(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Player name cannot be empty", nameof(name));
        }
        if (id < 0)
        {
            throw new ArgumentException("Player ID must be non-negative", nameof(id));
        }

        return new Player(id, name);
    }
}

