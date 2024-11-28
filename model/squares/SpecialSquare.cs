public class SpecialSquare : Square {
    private bool _opened = false;

    public override bool Opened { get {
        return _opened;
    }}

    public SpecialSquareType Type {get;}

    public SpecialSquare(SpecialSquareType type) {
        Type = type;
    }

    public void Open() {
        _opened = true;
    }
}

    