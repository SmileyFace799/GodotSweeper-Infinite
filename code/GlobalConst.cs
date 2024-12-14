using System;
using System.Reflection.Metadata;

public class GlobalConst {
    public const string SIGNAL_PAUSE = "Pause";
    public const string SIGNAL_GAME_OVER = "GameOver";

    private GlobalConst() {
        throw new InvalidOperationException("Utility class");
    }
}