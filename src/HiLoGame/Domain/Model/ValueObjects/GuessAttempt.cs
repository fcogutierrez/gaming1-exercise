using Domain.Model.Enums;

namespace Domain.Model.ValueObjects;

internal sealed class GuessAttempt
{
    public int Guess { get; }
    public PlayerGuessStatus Status { get; }

    private GuessAttempt(int guess, PlayerGuessStatus status)
    {
        Guess = guess;
        Status = status;
    }

    public static GuessAttempt Create(int guess, MisteryNumber misteryNumber)
    {
        if (guess < 0)
        {
            throw new ArgumentException("Guess must be non-negative", nameof(guess));
        }

        PlayerGuessStatus status;
        if (guess == misteryNumber.Value)
        {
            status = PlayerGuessStatus.Correct;
        }
        else if (guess < misteryNumber.Value)
        {
            status = PlayerGuessStatus.TooLow;
        }
        else
        {
            status = PlayerGuessStatus.TooHigh;
        }

        return new GuessAttempt(guess, status);
    }
}