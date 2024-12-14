public class StatsModel {
    public int Lives {get; set;}
    public double MinechanceReduction {get; set;}
    public bool Alive => Lives > 0;

    public StatsModel(int lives, double minechanceReduction) {
        Lives = lives;
        MinechanceReduction = minechanceReduction;
    }

    public static StatsModel GetStartingStats() {
        return new StatsModel(
            lives: 3,
            minechanceReduction: 0
        );
    }
}