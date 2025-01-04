using System;
using Godot;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.Godot
{
    public static class Utils
    {
        private const int ATLAS_WIDTH = 7;

        public static Position ToPosition(Vector2I vector) => new Position{X=vector.X, Y=vector.Y};
        public static Vector2I ToVector2I(Position position) => new((int) position.X, (int) position.Y);

        private static TileName GetTileName(SquareType type) => type.Switch(new() {
            {SpecialSquareType.BOMB, TileName.BOMB_EXPLODED},
            {SpecialSquareType.LIFE, TileName.EXTRA_LIFE},
            {SpecialSquareType.BAD_CHANCE_MODIFIER, TileName.MINECHANCE_REDUCTION_MEDIUM},
            {NumberSquareType.DEFAULT, TileName.DEFAULT_ZERO},
            {NumberSquareType.CROSS, TileName.CROSS_ZERO},
            {NumberSquareType.PLUS, TileName.PLUS_ZERO},
            {NumberSquareType.DIAMOND, TileName.DIAMOND_ZERO},
            {NumberSquareType.DEFUSED_BOMB, TileName.DEFUSED_ZERO},
            {PowerUpSquareType.SOLVER_SMALL, TileName.SOLVER_SMALL_TILE},
            {PowerUpSquareType.SOLVER_MEDIUM, TileName.SOLVER_MEDIUM_TILE},
            {PowerUpSquareType.SOLVER_LARGE, TileName.SOLVER_LARGE_TILE},
            {PowerUpSquareType.DEFUSER, TileName.DEFUSER_TILE}
        }, TileName.MISSING_TEXTURE);

        public static Vector2I GetAtlasCoords(ImmutableSquare square)
        {
            Vector2I atlasCoords;
            if (square.Flagged) {
                atlasCoords = GetAtlasCoords(TileName.FLAGGED);
            } else if (!square.Opened) {
                atlasCoords = GetAtlasCoords(TileName.HIDDEN);
            } else if (square is NumberSquare numberSquare) {
                atlasCoords = GetAtlasCoords(numberSquare.Type, numberSquare.Number);
            } else if (square is SpecialSquare specialSquare) {
                atlasCoords = GetAtlasCoords(specialSquare.Type);
            } else {
                atlasCoords = new(-1, -1);
            }
            return atlasCoords;
        }

        private static Vector2I GetAtlasCoords(NumberSquareType type, int number) {
            if (number < 0 || number > type.RelativeCoverage.Length) {
                throw new ArgumentException($"{number} is not a valid number for a square of this type.");
            }
            return GetAtlasCoords((TileName) ((int) GetTileName(type) + number));
        }
        private static Vector2I GetAtlasCoords(SpecialSquareType type) => GetAtlasCoords(GetTileName(type));
        private static Vector2I GetAtlasCoords(TileName tileName) => new((int) tileName % ATLAS_WIDTH, (int) tileName / ATLAS_WIDTH);

        public enum TileName {
            DEFAULT_ZERO,
            DEFAULT_ONE,
            DEFAULT_TWO,
            DEFAULT_THREE,
            DEFAULT_FOUR,
            DEFAULT_FIVE,
            DEFAULT_SIX,
            DEFAULT_SEVEN,
            DEFAULT_EIGHT,
            BOMB_EXPLODED,
            BOMB_REVEALED,
            FLAGGED,
            FLAGGED_WRONG,
            HIDDEN,
            CROSS_ZERO,
            CROSS_ONE,
            CROSS_TWO,
            CROSS_THREE,
            CROSS_FOUR,
            CROSS_FIVE,
            CROSS_SIX,
            CROSS_SEVEN,
            CROSS_EIGHT,
            PLUS_ZERO,
            PLUS_ONE,
            PLUS_TWO,
            PLUS_THREE,
            PLUS_FOUR,
            PLUS_FIVE,
            PLUS_SIX,
            PLUS_SEVEN,
            PLUS_EIGHT,
            DIAMOND_ZERO,
            DIAMOND_ONE,
            DIAMOND_TWO,
            DIAMOND_THREE,
            DIAMOND_FOUR,
            DIAMOND_FIVE,
            DIAMOND_SIX,
            DIAMOND_SEVEN,
            DIAMOND_EIGHT,
            MISSING_TEXTURE,
            MINECHANCE_REDUCTION_SMALL,
            MINECHANCE_REDUCTION_MEDIUM,
            MINECHANCE_REDUCTION_LARGE,
            EXTRA_LIFE,
            SOLVER_SMALL_TILE,
            SOLVER_MEDIUM_TILE,
            SOLVER_LARGE_TILE,
            VOID_SMALL_TILE,
            VOID_MEDIUM_TILE,
            VOID_LARGE_TILE,
            DEFUSER_TILE,
            DEFUSED_ZERO,
            DEFUSED_ONE,
            DEFUSED_TWO,
            DEFUSED_THREE,
            DEFUSED_FOUR,
            DEFUSED_FIVE,
            DEFUSED_SIX,
            DEFUSED_SEVEN,
            DEFUSED_EIGHT
        }
    }
}