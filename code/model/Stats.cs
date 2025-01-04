namespace SmileyFace799.RogueSweeper.model
{
    public interface ImmutableStats
    {
        int Lives {get;}
        int LivesGained {get;}
        int LivesLost {get;}
        double BadChanceModifier {get;}
        bool Alive {get;}
        ulong OpenedSquares {get;}
        uint SmallSolvers {get;}
        uint MediumSolvers {get;}
        uint LargeSolvers {get;}
        uint Defusers {get;}
    }

    public class Stats : ImmutableStats
    {
        private int _livesGained;
        private int _livesLost;
        private int _startingLives;

        public int Lives {get => _startingLives + _livesGained - _livesLost; set {
            int change = value - Lives;
            if (change > 0) {
                _livesGained += change;
            } else if (change < 0) {
                _livesLost -= change;
            }
        }}
        public int LivesGained => _livesGained;
        public int LivesLost => _livesLost;
        public double BadChanceModifier {get; set;}
        public bool Alive => Lives > 0;
        public ulong OpenedSquares {get; set;}
        public uint SmallSolvers {get; set;}
        public uint MediumSolvers {get; set;}
        public uint LargeSolvers {get; set;}
        public uint Defusers {get; set;}

        public Stats(
            int startingLives=1,
            int livesGained=0,
            int livesLost=0,
            double minechanceReduction=0,
            ulong openedSquares=0,
            uint smallSolvers=0,
            uint mediumSolvers=0,
            uint largeSolvers=0,
            uint defusers=0
        ) {
            _livesGained = livesGained;
            _livesLost = livesLost;
            _startingLives = startingLives;

            BadChanceModifier = minechanceReduction;
            OpenedSquares = openedSquares;
            SmallSolvers = smallSolvers;
            MediumSolvers = mediumSolvers;
            LargeSolvers = largeSolvers;
            Defusers = defusers;
        }

        public override string ToString() => GetType().Name + "{"
            + "Lives=" + Lives
            + ", BadChanceModifier=" + BadChanceModifier
            + ", Alive=" + Alive
            + ", OpenedSquares=" + OpenedSquares
            + ", SmallSolvers=" + SmallSolvers
            + ", MediumSolvers=" + MediumSolvers
            + ", LargeSolvers=" + LargeSolvers
            + ", Defusers=" + Defusers
            + "}";
    }
}