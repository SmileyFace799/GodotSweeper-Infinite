public abstract class Square {
    private bool _flagged;

    public bool Flagged {get {return _flagged;}}
    public abstract bool Opened {get;}

    public void ToggleFlagged() {
        _flagged = !_flagged;
    }
}