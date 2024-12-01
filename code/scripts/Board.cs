using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Board : TileMap, BoardUpdateListener {
	private const float CAMERA_MOVEMENT_INITIALIZE_RANGE = 15;
	private const float CMIR_SQUARED = CAMERA_MOVEMENT_INITIALIZE_RANGE * CAMERA_MOVEMENT_INITIALIZE_RANGE;
	private const float INITIAL_ZOOM_LEVEL = 1;
	private const float ZOOM_LEVEL_INCREMENT = 0.5F;

	private readonly BoardModel _model;
	private Camera2D _camera;

	/// <summary>
	/// Stores data on initiated camera movement. If this is <c>null</c>, there is no initiated camera movement, and the camera should not move.
	/// </summary>
	private CameraMoveData _cameraMoveData = null;
	/// <summary>
	/// An arbitrary, but linear "zoom level". Incrementing this will zoom in, while decrementing it will zoom out.
	/// </summary>
	private float _camZoomLevel = INITIAL_ZOOM_LEVEL;
	/// <summary>
	/// Maps the linear zoom level to an actual multiplier.
	/// </summary>
	private Vector2 ActualZoom { get {
		float actualZoom = _camZoomLevel < 0 ? (1/(-_camZoomLevel + 1)) : (_camZoomLevel + 1);
		return new(actualZoom, actualZoom);
	}}

	private void IncrementZoom(bool inwards, Vector2 rawMousePos) {
		float increment = inwards ? ZOOM_LEVEL_INCREMENT : -ZOOM_LEVEL_INCREMENT;
		_camZoomLevel += increment;
		Vector2 oldMousePos = ToTopLeft(ToCamZoom(rawMousePos));
		_camera.Zoom = ActualZoom;
		_camera.Position -= ToTopLeft(ToCamZoom(rawMousePos) - oldMousePos);
	}

	public Board() : base() {
		_model = new();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_camera = GetViewport().GetCamera2D();
		if (_camera == null) {
			throw new InvalidOperationException("Could not find camera node");
		}
		_camera.Zoom = ActualZoom;

		_model.Listener = this;
		_model.RevealSquare(new(0, 0), 0, 0);
	}

	/// <summary>
	/// Utility function to check if a square at a specified position satisfies a specified condition.
	/// </summary>
	/// <param name="position">The position of the square to check</param>
	/// <param name="ifNotFound">The default result if the square doesn't exist</param>
	/// <param name="checker">The condition to check if the square exists</param>
	/// <returns>If the condition is satisfied or not, or the default result if the square didn't exist</returns>
	private bool SquareState(Position position, bool ifNotFound, Func<SquareModel, bool> checker) {
		bool state = _model.HasSquare(position) ^ ifNotFound;
		if (state ^ ifNotFound) { // Can only enter if HasSquare returns true
			state = checker(_model.GetSquare(position));
		}
		return state;
	}

	/// <summary>
	/// Checks if a square can be revealed at a specified position.
	/// </summary>
	/// <param name="position">The position to check</param>
	/// <returns>If a square can be revealed at the specified position</returns>
	private bool IsRevealable(Position position) => SquareState(position, true, s => !s.Flagged && !s.Opened);
	/// <summary>
	/// Checks if a square can be flagged at a specified position.
	/// </summary>
	/// <param name="position">The position to check</param>
	/// <returns>If a square can be flagged at the specified position</returns>
	private bool IsFlaggable(Position position) => SquareState(position, false, s => !s.Opened);
	/// <summary>
	/// Checks if a square can be "smart revealed" at a specified position.<br/>
	/// "Smart revealing" refers to revealing a number square's covered squares if its number equals the number of covered squares that are flagged.
	/// </summary>
	/// <param name="position">The position to check</param>
	/// <returns>If a square can be "smart revealed" at the specified position</returns>
	private bool IsSmartRevealable(Position position) => SquareState(position, false, s => s.Opened && s is NumberSquareModel);

	/// <summary>
	/// Transforms a position within the viewport to the equivalent position within the game world, when accounting for camera zoom.
	/// Note that the position is still relative to the viewport, use <see cref="ToAbsolute"/> to also transform the position to
	/// </summary>
	/// <param name="absoluteZoomPos">Te viewposrt position to transform</param>
	/// <returns>A zoom-adjusted equivalent of the provided viewport position</returns>
	private Vector2 ToCamZoom(Vector2 absoluteZoomPos) => GetViewport().CanvasTransform.AffineInverse().BasisXform(absoluteZoomPos);
	/// <summary>
	/// Offsets a point to account for the camera's anchor being the center
	/// </summary>
	/// <param name="middlePosition">The point to transform</param>
	/// <returns>A transformed point</returns>
	private Vector2 ToTopLeft(Vector2 middlePosition) => middlePosition - ToCamZoom(GetViewport().GetVisibleRect().Size / 2);
	/// <summary>
	/// Transforms a position relative to the viewport to a position relative to the game. 
	/// </summary>
	/// <param name="camRelativePos"></param>
	/// <returns>An absolute position in the game world</returns>
	private Vector2 ToAbsolute(Vector2 camRelativePos) => ToTopLeft(_camera.Position + ToCamZoom(camRelativePos));

    public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseMotion mouseMotionEvent && _cameraMoveData != null) {
			_cameraMoveData.UpdateInitialized(mouseMotionEvent.Position);
			if (_cameraMoveData.MoveInitiated) {
				_camera.Position = _cameraMoveData.OriginPoint - ToCamZoom(mouseMotionEvent.Position);
			}
		} else if (@event is InputEventMouseButton mouseEvent) {
			if (mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Middle) {
				_cameraMoveData = new(_camera.Position + ToCamZoom(mouseEvent.Position), mouseEvent.Position);
			} else if (mouseEvent.IsReleased()) {
				Vector2 pos = ToAbsolute(mouseEvent.Position);
				Position clickedBoardPosition = Utils.ToPosition(LocalToMap(pos));
				
				if (mouseEvent.ButtonIndex == MouseButton.Left && IsRevealable(clickedBoardPosition)) {
					_model.RevealSquare(clickedBoardPosition);
				} else if (mouseEvent.ButtonIndex == MouseButton.Right && IsFlaggable(clickedBoardPosition)) {
					_model.FlagSquare(clickedBoardPosition);
				} else if (mouseEvent.ButtonIndex == MouseButton.Middle) {
					if (IsSmartRevealable(clickedBoardPosition) && !_cameraMoveData.MoveInitiated) {
						NumberSquareModel numberSquare = (NumberSquareModel) _model.GetSquare(clickedBoardPosition);
						Dictionary<Position, SquareModel> coveredSquares = _model.GetOrGenerateCoveredSquares(clickedBoardPosition, numberSquare.Type);
						if (numberSquare.Number == coveredSquares.Values.Where(s => s.Flagged).Count()) {
							_model.RevealSquares(
								coveredSquares.Where(kvp => !kvp.Value.Flagged && !kvp.Value.Opened).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
								BoardModel.CASCADE_LIMIT,
								BoardModel.SPECIAL_BAD_CHANCE,
								BoardModel.SPECIAL_GOOD_CHANCE
							);
						}
					}
					_cameraMoveData = null;
				} else if (mouseEvent.ButtonIndex == MouseButton.WheelUp) {
					IncrementZoom(true, mouseEvent.Position);
				} else if (mouseEvent.ButtonIndex == MouseButton.WheelDown) {
					IncrementZoom(false, mouseEvent.Position);
				}
			}
		} else if (@event is InputEventKey keyEvent) {
			if (keyEvent.Keycode == Key.Escape) {
				_camera.Position = Vector2.Zero;
				_cameraMoveData = null;
			}
		}
		GetViewport().SetInputAsHandled();
    }

    public void OnSquareUpdated(Position position, SquareModel square) {
        SetCell(0, Utils.ToVector2I(position), 0, Utils.GetAtlasCoords(square));
    }

	private class CameraMoveData {
		private bool _moveInitiated = false;
		private readonly Vector2 _viewportOrigin;

		public Vector2 OriginPoint {get;}
		public bool MoveInitiated { get {return _moveInitiated;}}

		public CameraMoveData(Vector2 originPoint, Vector2 viewportOrigin) {
			OriginPoint = originPoint;
			_viewportOrigin = viewportOrigin;
		}

		public void UpdateInitialized(Vector2 position) {
			if (!MoveInitiated) {
				// LengthSquared is used since it is faster
				_moveInitiated = CMIR_SQUARED < (position - _viewportOrigin).LengthSquared();
			}
		}
	}
}
