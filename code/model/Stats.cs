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
        private const int STARTING_LIVES = 1;

        private int _livesGained;
        private int _livesLost;

        public int Lives {get => STARTING_LIVES + _livesGained - _livesLost; set {
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
            int livesGained=0,
            int livesLost=0,
            double badChanceModifier=0,
            ulong openedSquares=0,
            uint smallSolvers=0,
            uint mediumSolvers=0,
            uint largeSolvers=0,
            uint defusers=0
        ) {
            _livesGained = livesGained;
            _livesLost = livesLost;

            BadChanceModifier = badChanceModifier;
            OpenedSquares = openedSquares;
            SmallSolvers = smallSolvers;
            MediumSolvers = mediumSolvers;
            LargeSolvers = largeSolvers;
            Defusers = defusers;
        }

        public override string ToString() => GetType().Name + "{"
            + "LivesGained=" + LivesGained
            + ", LivesLost=" + LivesLost
            + ", BadChanceModifier=" + BadChanceModifier
            + ", OpenedSquares=" + OpenedSquares
            + ", SmallSolvers=" + SmallSolvers
            + ", MediumSolvers=" + MediumSolvers
            + ", LargeSolvers=" + LargeSolvers
            + ", Defusers=" + Defusers
            + "}";
    }
}