using System;

public class NumberSquareModel : SquareModel {
    private int _number = -1;

    public override bool Opened { get {
        return _number != -1;
    }}

    /// <summary>
    /// The actual number of this square. This must be assigned before it can be read,
    /// and it can be assigned only once. Assigning this is what "opens" the square.
    /// </summary>
    public int Number { get {
        if (!Opened) {
            throw new InvalidOperationException("This square is not opened & does not have a number yet");
        }
        return _number;
    } set {
        if (Opened) {
            throw new InvalidOperationException("This is already opened & has a number");
        } else if (value < 0 || value > Type.RelativeCoverage.Count) {
            throw new ArgumentOutOfRangeException($"The number must be between 0 & the square type's max coverage ({Type.RelativeCoverage.Count})");
        }
        _number = value;
    }}

    public override NumberSquareType Type {get;}

    public NumberSquareModel(NumberSquareType type) {
        Type = type;
    }

}