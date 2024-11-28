using System;
using System.Collections.Generic;
using System.Linq;

public abstract class SquareType {
    public abstract double Weight {get;}

    private static readonly Random RANDOM = new();

    protected static T GetRandom<T>(params T[] squareTypes) where T : SquareType {
        List<double> weights = squareTypes.Select(t => t.Weight).ToList();
        double random = RANDOM.NextDouble() * weights.Sum();
        int i = 0;
        while (random >= weights[i]) {
            random -= weights[i];
            ++i;
        }
        return squareTypes[i];
    }
}