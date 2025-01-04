namespace SmileyFace799.RogueSweeper.model
{
    public class SpecialSquareType : SquareType
    {
        public static readonly SpecialSquareType BOMB = new(1, TypeLevel.BAD);
        public static readonly SpecialSquareType LIFE = new(2, TypeLevel.GOOD);
        public static readonly SpecialSquareType BAD_CHANCE_MODIFIER = new(3, TypeLevel.GOOD);

        public override double Weight {get;}

        protected SpecialSquareType(double weight, TypeLevel level) : base(level)
        {
            Weight = weight;
        }
    }

    public class PowerUpSquareType : SpecialSquareType
    {
        public static readonly PowerUpSquareType SOLVER_SMALL = new(5, PowerUp.SOLVER_SMALL);
        public static readonly PowerUpSquareType SOLVER_MEDIUM = new(3, PowerUp.SOLVER_MEDIUM);
        public static readonly PowerUpSquareType SOLVER_LARGE = new(1, PowerUp.SOLVER_LARGE);
        public static readonly PowerUpSquareType DEFUSER = new(4, PowerUp.DEFUSER);

        public PowerUp PowerUp {get;}

        private PowerUpSquareType(double weight, PowerUp powerUp) : base(weight, TypeLevel.GOOD)
        {
            PowerUp = powerUp;
        }
    }
}