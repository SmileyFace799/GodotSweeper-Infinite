using System;
using System.Linq;
using Godot;

public class Utils {
    private const int ATLAS_WIDTH = 7;

    private Utils() => throw new InvalidOperationException("Utility class");

    public static Position ToPosition(Vector2I vector) {
        return new Position{X=vector.X, Y=vector.Y};
    }

    public static Vector2I ToVector2I(Position position) {
        return new((int) position.X, (int) position.Y);
    }

    public static Vector2I GetAtlasCoords(SquareModel square) {
        Vector2I atlasCoords;
        if (square.Flagged) {
            atlasCoords = GetAtlasCoords(TileName.FLAGGED);
        } else if (!square.Opened) {
			atlasCoords = GetAtlasCoords(TileName.HIDDEN);
		} else if (square is NumberSquareModel numberSquare) {
			atlasCoords = GetAtlasCoords(numberSquare.Type, numberSquare.Number);
		} else if (square is SpecialSquareModel specialSquare) {
			atlasCoords = GetAtlasCoords(specialSquare.Type);
		} else {
            atlasCoords = new(-1, -1);
        }
        return atlasCoords;
    }

    private static Vector2I GetAtlasCoords(NumberSquareType type, int number) {
        if (number < 0 || number > type.RelativeCoverage.Count()) {
            throw new ArgumentException($"{number} is not a valid number for a square of this type.");
        }
        TileName zero;
        if (type == NumberSquareType.DEFAULT) {
            zero = TileName.DEFAULT_ZERO;
        } else if (type == NumberSquareType.CROSS) {
            zero = TileName.CROSS_ZERO;
        } else if (type == NumberSquareType.PLUS) {
            zero = TileName.PLUS_ZERO;
        } else if (type == NumberSquareType.DIAMOND) {
            zero = TileName.DIAMOND_ZERO;
        } else {
            zero = TileName.MISSING_TEXTURE;
            number = 0;
        }
        return GetAtlasCoords((TileName) ((int) zero + number));
    }

    private static Vector2I GetAtlasCoords(SpecialSquareType type) {
        TileName name;
        if (type == SpecialSquareType.BOMB) {
            name = TileName.BOMB_EXPLODED;
        } else if (type == SpecialSquareType.LIFE) {
            name = TileName.EXTRA_LIFE;
        } else if (type == SpecialSquareType.MINECHANCE_REDUCTION) {
            name = TileName.MINECHANCE_REDUCTION_MEDIUM;
        } else {
            name = TileName.MISSING_TEXTURE;
        }
        return GetAtlasCoords(name);
    }

    private static Vector2I GetAtlasCoords(TileName tileName) {
        return new((int) tileName % ATLAS_WIDTH, (int) tileName / ATLAS_WIDTH);
    }

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
        EXTRA_LIFE
    }
}