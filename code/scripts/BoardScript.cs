using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Godot;
using SmileyFace799.RogueSweeper.events;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.Godot
{
	public partial class BoardScript : TileMap, IUIEventReceiver
	{
		private const float CAMERA_MOVEMENT_INITIALIZE_RANGE = 15;
		private const float CMIR_SQUARED = CAMERA_MOVEMENT_INITIALIZE_RANGE * CAMERA_MOVEMENT_INITIALIZE_RANGE;
		private const float INITIAL_ZOOM_LEVEL = 1;
		private const float ZOOM_LEVEL_INCREMENT = 0.5F;
		private const string SIGNAL_HOVERED_SQUARE_UPDATED = "HoveredSquareUpdated";
		private const string SIGNAL_TOGGLE_PAUSED = "TogglePaused";
		private const string SIGNAL_GAME_OVER = "GameOver";

		private Camera2D _camera;
		private bool _boardInitiated;
		private Position? _lastHovered = null;

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

		[Signal]
		public delegate void HoveredSquareUpdatedEventHandler(long x, long y);

		[Signal]
		public delegate void TogglePausedEventHandler();

		[Signal]
		public delegate void GameOverEventHandler();

		private void IncrementZoom(bool inwards, Vector2 rawMousePos)
		{
			float increment = inwards ? ZOOM_LEVEL_INCREMENT : -ZOOM_LEVEL_INCREMENT;
			_camZoomLevel += increment;
			Vector2 oldMousePos = ToTopLeft(ToCamZoom(rawMousePos));
			_camera.Zoom = ActualZoom;
			_camera.Position -= ToTopLeft(ToCamZoom(rawMousePos) - oldMousePos);
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_camera = GetViewport().GetCamera2D();
			if (_camera == null) {
				throw new InvalidOperationException("Could not find camera node");
			}
			_camera.Zoom = ActualZoom;

			Game.Instance.SelectedPowerUp = null;
			Game.Instance.AddReceiver(this);
		}

        public override void _Notification(int what)
        {
			if (what == NotificationPredelete) {
				Game.Instance.RemoveReceiver(this);
				GD.Print("Unloaded board script");
			}
        }

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

		public override void _ShortcutInput(InputEvent @event)
		{
			if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.Escape) {
				if ((keyEvent.GetModifiersMask() & (KeyModifierMask.MaskShift | KeyModifierMask.MaskCtrl)) != 0) { // Checks if Shift or Ctrl keymod is present
					_camera.Position = Vector2.Zero;
					_cameraMoveData = null;
				} else if (Game.Instance.Alive) {
					EmitSignal(SIGNAL_TOGGLE_PAUSED);
				} else {
					EmitSignal(SIGNAL_GAME_OVER);
				}
				GetViewport().SetInputAsHandled();
			}
		}

		private Position ToBoardPosition(Vector2 screenPosition) => Utils.ToPosition(LocalToMap(ToAbsolute(screenPosition)));

		private void HandleSquareInteraction(InputEventMouseButton mouseEvent)
		{
			Position clickedBoardPosition = ToBoardPosition(mouseEvent.Position);

			switch (mouseEvent.ButtonIndex) {
				case MouseButton.Left:
					Game.Instance.LeftClick(clickedBoardPosition);
					break;
				case MouseButton.Right:
					Game.Instance.RightClick(clickedBoardPosition);
					break;
				case MouseButton.Middle:
					Game.Instance.MiddleClick(clickedBoardPosition);
					break;
			}
		}

		private void HandleMouseMotionEvent(InputEventMouseMotion mouseMotionEvent)
		{
			if (_cameraMoveData == null) {
				Position boardPosition = ToBoardPosition(mouseMotionEvent.Position);
				if (_lastHovered != boardPosition) {
					_lastHovered = boardPosition;
					EmitSignal(SIGNAL_HOVERED_SQUARE_UPDATED, boardPosition.X, boardPosition.Y);
				}
			} else {
				_cameraMoveData.UpdateInitialized(mouseMotionEvent.Position);
				if (_cameraMoveData.MoveInitiated) {
					_camera.Position = _cameraMoveData.OriginPoint - ToCamZoom(mouseMotionEvent.Position);
				}
			}
		}

		private void HandleMouseButtonEvent(InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.IsPressed() && mouseButtonEvent.ButtonIndex == MouseButton.Middle) {
				_cameraMoveData = new(_camera.Position + ToCamZoom(mouseButtonEvent.Position), mouseButtonEvent.Position);
			} else if (mouseButtonEvent.IsReleased()) {
				bool camMovementWasStopped = false;
				if (mouseButtonEvent.ButtonIndex == MouseButton.Middle) {
					camMovementWasStopped = _cameraMoveData != null && _cameraMoveData.MoveInitiated;
					_cameraMoveData = null;
				}

				if (mouseButtonEvent.ButtonIndex == MouseButton.WheelUp) {
					IncrementZoom(true, mouseButtonEvent.Position);
				} else if (mouseButtonEvent.ButtonIndex == MouseButton.WheelDown) {
					IncrementZoom(false, mouseButtonEvent.Position);
				} else if (!camMovementWasStopped && Game.Instance.Alive) {
					HandleSquareInteraction(mouseButtonEvent);
				}
			}
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventMouseMotion mouseMotionEvent) {
				HandleMouseMotionEvent(mouseMotionEvent);
			} else if (@event is InputEventMouseButton mouseButtonEvent) {
				HandleMouseButtonEvent(mouseButtonEvent);
			}
			GetViewport().SetInputAsHandled();
		}

		public void OnNewGameLoaded(NewGameLoadedEvent @event)
		{
			foreach ((long x, ConcurrentDictionary<long, Square> column) in @event.Squares) {
				foreach ((long y, IImmutableSquare square) in column) { // Cast to ImmutableSquare ensures it's not modified
					UpdateSquare(new(x, y), square, TaskPriority.SQUARE_LOADED);
				}
			}
		}

		private void UpdateSquare(Position position, IImmutableSquare square, int priority) => GDThread.QueueTask(priority, () => SetCell(0, Utils.ToVector2I(position), 0, Utils.GetAtlasCoords(square)));

		public void OnSquareUpdated(SquareUpdatedEvent @event) => UpdateSquare(@event.Position, @event.Square, @event.Priority);

		public void OnUpdateUI(IUIUpdateEvent @event) {
			switch (@event) {
				case NewGameLoadedEvent nglEvent:
					OnNewGameLoaded(nglEvent);
					break;
				case SquareUpdatedEvent suEvent:
					OnSquareUpdated(suEvent);
					break;
				case GameRestartedEvent:
					GDThread.QueueTask(TaskPriority.SCENE_CHANGE, () => {
						GetTree().Paused = false;
						GetTree().ReloadCurrentScene();
					});
					break;
			}
		}

		private class CameraMoveData
		{
			private bool _moveInitiated = false;
			private readonly Vector2 _viewportOrigin;

			public Vector2 OriginPoint {get;}
			public bool MoveInitiated { get {return _moveInitiated;}}

			public CameraMoveData(Vector2 originPoint, Vector2 viewportOrigin)
			{
				OriginPoint = originPoint;
				_viewportOrigin = viewportOrigin;
			}

			public void UpdateInitialized(Vector2 position)
			{
				if (!MoveInitiated) {
					// LengthSquared is used since it is faster
					_moveInitiated = CMIR_SQUARED < (position - _viewportOrigin).LengthSquared();
				}
			}
		}
	}
}