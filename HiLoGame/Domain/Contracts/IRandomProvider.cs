namespace Domain.Contracts;

public interface IRandomProvider
{
    int GetRandomNumber(int min, int max);
}
