public abstract class SquareModel {
    private bool _flagged;

    public bool Flagged {get {return _flagged;}}
    public abstract bool Opened {get;}
    public abstract SquareType Type {get;}

    public void ToggleFlagged() {
        _flagged = !_flagged;
    }
}