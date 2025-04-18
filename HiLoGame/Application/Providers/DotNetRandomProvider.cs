using Domain.Contracts;
using System;

namespace Application.Providers;

internal sealed class DotNetRandomProvider : IRandomProvider
{
    public int GetRandomNumber(int min, int max)
    {
        var random = new Random();
        var randomNumber = random.Next(min, max + 1);

        return randomNumber;
    }
}
