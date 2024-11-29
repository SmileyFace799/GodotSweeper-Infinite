using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Board : TileMap, BoardUpdateListener {
	private BoardModel model;

	public Board() : base() {
		model = new();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		model.Listener = this;
		model.RevealSquare(new(16, 9), 0, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	private bool SquareState(Position position, bool ifNotFound, Func<SquareModel, bool> checker) {
		bool state = model.HasSquare(position) ^ ifNotFound;
		if (state ^ ifNotFound) { // Can only enter if HasSquare returns true
			state = checker(model.GetSquare(position));
		}
		return state;
	}

	private bool IsRevealable(Position position) => SquareState(position, true, s => !s.Flagged && !s.Opened);
	private bool IsFlaggable(Position position) => SquareState(position, false, s => !s.Opened);

	private bool IsSmartRevealable(Position position) => SquareState(position, false, s => s.Opened && s is NumberSquareModel);

    public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsReleased()) {
			Vector2 pos = GetViewport().CanvasTransform.AffineInverse().BasisXform(mouseEvent.Position);
			Position clickedBoardPosition = Utils.ToPosition(LocalToMap(pos));
			
			if (mouseEvent.ButtonIndex == MouseButton.Left && IsRevealable(clickedBoardPosition)) {
				model.RevealSquare(clickedBoardPosition);
			} else if (mouseEvent.ButtonIndex == MouseButton.Right && IsFlaggable(clickedBoardPosition)) {
				model.FlagSquare(clickedBoardPosition);
			} else if (mouseEvent.ButtonIndex == MouseButton.Middle && IsSmartRevealable(clickedBoardPosition)) {
				NumberSquareModel numberSquare = (NumberSquareModel) model.GetSquare(clickedBoardPosition);
				Dictionary<Position, SquareModel> coveredSquares = model.GetOrGenerateCoveredSquares(clickedBoardPosition, numberSquare.Type);
				if (numberSquare.Number == coveredSquares.Values.Where(s => s.Flagged).Count()) {
					model.RevealSquares(
						coveredSquares.Where(kvp => !kvp.Value.Flagged && !kvp.Value.Opened).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
						BoardModel.CASCADE_LIMIT,
						BoardModel.SPECIAL_BAD_CHANCE,
						BoardModel.SPECIAL_GOOD_CHANCE
					);
				}
			}
			GetViewport().SetInputAsHandled();
		}
    }

    public void OnSquareUpdated(Position position, SquareModel square) {
        SetCell(0, Utils.ToVector2I(position), 0, Utils.GetAtlasCoords(square));
    }
}
