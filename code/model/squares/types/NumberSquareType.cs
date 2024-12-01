using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Contains static instances of all square types:
/// <c>DEFAULT</c>, <c>CROSS</c>, <c>PLUS</c> &amp; <c>DIAMOND</c>.
/// </summary>
public class NumberSquareType : SquareType {
    public static readonly NumberSquareType DEFAULT = new(
        2,
        new(-1, -1),
        new(0, -1),
        new(1, -1),
        new(-1, 0),
        new(1, 0),
        new(-1, 1),
        new(0, 1),
        new(1, 1)
    );

    public static readonly NumberSquareType CROSS = new(
        0,
        new(-2, -2),
        new(2, -2),
        new(-1, -1),
        new(1, -1),
        new(-1, 1),
        new(1, 1),
        new(-2, 2),
        new(2, 2)
    );

    public static readonly NumberSquareType PLUS = new(
        0,
        new(0, -2),
        new(0, -1),
        new(-2, 0),
        new(-1, 0),
        new(1, 0),
        new(2, 0),
        new(0, 1),
        new(0, 2)
    );

    public static readonly NumberSquareType DIAMOND = new(
        0,
        new(0, -2),
        new(-1, -1),
        new(1, -1),
        new(-2, 0),
        new(2, 0),
        new(-1, 1),
        new(1, 1),
        new(0, 2)
    );

    public static readonly NumberSquareType[] ALL = new NumberSquareType[]{DEFAULT, CROSS, PLUS, DIAMOND};

    /// <summary>
    /// The set of relative positions this number type covers when counting bombs.
    /// </summary>
    public HashSet<Position> RelativeCoverage {get;}

    public override double Weight {get;}

    private NumberSquareType(double weight, params Position[] relativeCoverage) {
        Weight = weight;
        RelativeCoverage = relativeCoverage.ToHashSet();
    }

    public static NumberSquareType GetRandom() {
        return GetRandom(ALL);
    }
}