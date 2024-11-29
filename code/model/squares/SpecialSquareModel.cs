public class SpecialSquareModel : SquareModel {
    private bool _opened = false;

    public override bool Opened { get {
        return _opened;
    }}

    public override SpecialSquareType Type {get;}

    public SpecialSquareModel(SpecialSquareType type) {
        Type = type;
    }

    public void Open() {
        _opened = true;
    }
}

    