using System;
using System.Linq;

namespace SmileyFace799.RogueSweeper.model {
    public static class SquareFactory
    {
        private static readonly Random RANDOM = new();
        private static readonly NumberSquareType[] NUMBERS = new NumberSquareType[] {
            NumberSquareType.DEFAULT,
            NumberSquareType.CROSS,
            NumberSquareType.PLUS,
            NumberSquareType.DIAMOND,
            NumberSquareType.DEFUSED_BOMB
        };
        private static readonly PowerUpSquareType[] POWER_UPS = new PowerUpSquareType[] {
            PowerUpSquareType.SOLVER_SMALL,
            PowerUpSquareType.SOLVER_MEDIUM,
            PowerUpSquareType.SOLVER_LARGE,
            PowerUpSquareType.DEFUSER
        };
        private static readonly SpecialSquareType[] SPECIALS = new SpecialSquareType[] {
            SpecialSquareType.BOMB,
            SpecialSquareType.LIFE,
            SpecialSquareType.BAD_CHANCE_MODIFIER
        };
        public static readonly SquareType[] ALL = ((SquareType[]) NUMBERS).Concat(POWER_UPS).Concat(SPECIALS).ToArray();

        private static SquareType GetRandom(params SquareType[] squareTypes)
        {
            double[] weights = squareTypes.Select(t => t.Weight).ToArray();
            double random = RANDOM.NextDouble() * weights.Sum();
            int i = 0;
            while (random >= weights[i]) {
                random -= weights[i];
                ++i;
            }
            return squareTypes[i];
        }

        public static Square MakeRandom(TypeLevel levelFilter)
        {
            SquareType[] eligibleTypes = ALL.Where(t => t.Level == levelFilter).ToArray();
            if (eligibleTypes.Length == 0) {
                throw new ArgumentException("No squares have the provided level filter");
            }
            return GetRandom(eligibleTypes) switch {
                SpecialSquareType specialType => new SpecialSquare(specialType),
                NumberSquareType numberType => new NumberSquare(numberType),
                _ => throw new InvalidOperationException("Attempted to make a square of an unknown type")
            };
        }

        public static Square MakeRandom(Position position, SquareGenData squareGenData)
        {
            Square generatedSquare;
            double randomNum = RANDOM.NextDouble();
            double badChance = Math.Max(0, squareGenData.GetBadChance(position));
            double goodChance = Math.Max(0, squareGenData.GetGoodChance(position));
            if (randomNum < badChance) {
                generatedSquare = MakeRandom(TypeLevel.BAD);
            } else if (randomNum - badChance < goodChance) {
                generatedSquare = MakeRandom(TypeLevel.GOOD);
            } else {
                generatedSquare = MakeRandom(TypeLevel.NEUTRAL);
            }
            return generatedSquare;
        }
    }
}