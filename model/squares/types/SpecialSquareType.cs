using System;

public class SpecialSquareType : SquareType {
    public bool IsBad {get;}
    public override double Weight {get;}

    public static readonly SpecialSquareType BOMB = new(1, true);
    public static readonly SpecialSquareType LIFE = new(1, false);
    public static readonly SpecialSquareType MINECHANCE_REDUCTION = new(1, false);

    public static readonly SpecialSquareType[] ALL_BAD = new SpecialSquareType[]{BOMB};
    public static readonly SpecialSquareType[] ALL_GOOD = new SpecialSquareType[]{LIFE, MINECHANCE_REDUCTION};

    private SpecialSquareType(double weight, bool isBad) {
        Weight = weight;
        IsBad = isBad;
    }

    public static SpecialSquareType GetRandomBad() {
        return GetRandom(ALL_BAD);
    }

    public static SpecialSquareType GetRandomGood() {
        return GetRandom(ALL_GOOD);
    }
}