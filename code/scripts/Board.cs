using Godot;

public partial class Board : TileMap, BoardUpdateListener {
	private BoardModel model;

	public Board() : base() {
		model = new();
		model.Listener = this;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

    public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsReleased()) {
			Vector2 pos = GetViewport().CanvasTransform.AffineInverse().BasisXform(mouseEvent.Position);
			Position coordinates = Utils.ToPosition(LocalToMap(pos));
			if (!model.hasSquare(coordinates) || !model.GetSquare(coordinates).Opened) {
				model.RevealSquare(coordinates);
			}
			GetViewport().SetInputAsHandled();
		}
    }

    public void OnSquareUpdated(Position position, SquareModel square) {
        SetCell(0, Utils.ToVector2I(position), 0, Utils.GetAtlasCoords(square));
    }
}
