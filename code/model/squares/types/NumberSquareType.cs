namespace SmileyFace799.RogueSweeper.model
{
    /// <summary>
    /// Contains static instances of all square types:
    /// <c>DEFAULT</c>, <c>CROSS</c>, <c>PLUS</c> &amp; <c>DIAMOND</c>.
    /// </summary>
    public class NumberSquareType : SquareType
    {
        public static readonly NumberSquareType DEFAULT = new(
            2,
            TypeLevel.NEUTRAL,
            Position.Array2D(-1, -1, 3, 3, false)
        );

        public static readonly NumberSquareType DEFUSED_BOMB = new(
            0,
            TypeLevel.BAD,
            DEFAULT.RelativeCoverage
        );

        public static readonly NumberSquareType CROSS = new(
            0,
            TypeLevel.NEUTRAL,
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
            TypeLevel.NEUTRAL,
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
            TypeLevel.NEUTRAL,
            new(0, -2),
            new(-1, -1),
            new(1, -1),
            new(-2, 0),
            new(2, 0),
            new(-1, 1),
            new(1, 1),
            new(0, 2)
        );

        /// <summary>
        /// The set of relative positions this number type covers when counting bombs.
        /// </summary>
        public Position[] RelativeCoverage {get;}

        public override double Weight {get;}

        private NumberSquareType(double weight, TypeLevel level, params Position[] relativeCoverage) : base(level)
        {
            Weight = weight;
            RelativeCoverage = relativeCoverage;
        }
    }
}