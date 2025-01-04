namespace SmileyFace799.RogueSweeper.model {
public interface ImmutableSpecialSquare : ImmutableSquare {
    new SpecialSquareType Type {get;}
}

public class SpecialSquare : Square, ImmutableSpecialSquare {
    private bool _opened = false;

    public override bool Opened { get {
        return _opened;
    }}

    public override SpecialSquareType Type {get;}

    public SpecialSquare(SpecialSquareType type) {
        Type = type;
    }

    public void Open() {
        _opened = true;
    }
}
}