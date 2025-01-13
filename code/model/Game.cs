using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SmileyFace799.RogueSweeper.events;
using SmileyFace799.RogueSweeper.filestorage;
using SmileyFace799.RogueSweeper.Godot;
using SmileyFace799.RogueSweeper.Threading;

namespace SmileyFace799.RogueSweeper.model
{
	public class Game : BoardUpdateAdapter
	{
		public static Game Instance {get;} = new();

		private const string BOARD_FILENAME = ".board";
		private const string STATS_FILENAME = ".stats";

		private readonly BoardInterface _board;
		private readonly StatsInterface _stats;
		public SquareGenData StandardGenData => new RelativeGenData(_stats.StatsView.BadChanceModifier);

		private HashSet<IUIEventReceiver> _UIEventReceivers = new();

		public bool Alive => _stats.StatsView.Alive;
		public bool CanQuitSafely => !_stats.IsReadWriting && !_board.IsReadWriting;
		public PowerUp SelectedPowerUp {get; set;} = null;

		/// <summary>
		/// Creates a game from the stored .board save file, or starts a new game if the file doesn't exist.
		/// Also loads in stats
		/// </summary>
		private Game()
		{
			_board = new(".board");
			_stats = new(".stats");

			if (!FileInterface.ExistsAll(BOARD_FILENAME, STATS_FILENAME)) {
				Restart();
			} else {
				_board.BoardView.Listener = this;
			}

		}

		private void NotifyReceivers(IUIUpdateEvent @event)
		{
			foreach (IUIEventReceiver receiver in _UIEventReceivers) {
				receiver.OnUpdateUI(@event);
			}
		}

		public void AddReceiver(IUIEventReceiver receiver)
		{
			_UIEventReceivers.Add(receiver);
			_board.With(board => {
				receiver.OnUpdateUI(new NewGameLoadedEvent(_stats.StatsView, board.GetSquares()));
			});
		}

		public void RemoveReceiver(IUIEventReceiver receiver) {
			_UIEventReceivers.Remove(receiver);
		}

		/// <summary>
		/// <para>Resets the game state to a blank, new game.</para>
		/// </summary>
		public void Restart() {
			_board.ResetValue();
			_board.BoardView.Listener = this;
			_stats.ResetValue();
			GDThread.ClearTasks();
			NotifyReceivers(new GameRestartedEvent());
			_board.With(board => board.RevealSquare(new(0, 0), new StaticGenData(0, 0), StandardGenData));

		}

		public void LeftClick(Position boardPosition)
		{
			Tasks.Queue(() => {
				if (SelectedPowerUp == null) {
					if (_board.BoardView.IsRevealable(boardPosition)) {
						_board.With(board => board.RevealSquare(boardPosition, StandardGenData));
					}
				} else {
					_board.With(Board => {
						SelectedPowerUp.Action(Board, boardPosition, StandardGenData);
						_stats.With(stats => {
							SelectedPowerUp.Switch(new() {
								{PowerUp.SOLVER_SMALL, () => NotifyReceivers(new SmallSolversUpdatedEvent(--stats.SmallSolvers))},
								{PowerUp.SOLVER_MEDIUM, () => NotifyReceivers(new MediumSolversUpdatedEvent(--stats.MediumSolvers))},
								{PowerUp.SOLVER_LARGE, () => NotifyReceivers(new LargeSolversUpdatedEvent(--stats.LargeSolvers))},
								{PowerUp.DEFUSER, () => NotifyReceivers(new DefusersUpdatedEvent(--stats.Defusers))}
							});
						});
						SelectedPowerUp = null;
						NotifyReceivers(new PowerUpDeselectedEvent());
					});
				}
			});
		}

		public void RightClick(Position boardPosition)
		{
			Tasks.Queue(() => {
				if (SelectedPowerUp == null) {
					if (_board.BoardView.IsFlaggable(boardPosition)) {
						_board.With(board => {
							board.FlagSquare(boardPosition);
						});
					}
				} else {
					SelectedPowerUp = null;
					NotifyReceivers(new PowerUpDeselectedEvent());
				}
			});
		}

		public void MiddleClick(Position boardPosition)
		{
			Tasks.Queue(() => {
				if (SelectedPowerUp == null && _board.BoardView.IsSmartRevealable(boardPosition)){
					_board.With(board => {
						SquareGenData genData = StandardGenData;
						NumberSquare numberSquare = (NumberSquare) board.GetSquare(boardPosition);
						Dictionary<Position, Square> coveredSquares = board.GetOrGenerateCoveredSquares(boardPosition, numberSquare.Type, genData, SquaresUpdatedEvent.OPENED_PRIORITY - 1);
						if (numberSquare.Number == coveredSquares.Values.Where(s => s.Flagged || (s.Type.Level == TypeLevel.BAD && s.Opened)).Count()) {
							board.RevealSquares(
								coveredSquares.Where(kvp => !kvp.Value.Flagged && !kvp.Value.Opened).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
								Board.CASCADE_LIMIT,
								genData
							);
						}
					});
				}
			});
		}

		public override void OnSquareUpdated(SquaresUpdatedEvent @event) {
			_stats.With(stats => {
				foreach ((Position position, IImmutableSquare square) in @event.Squares) {
					if (square.Opened) {
						square.Type.Switch(new() {
							{SpecialSquareType.BOMB, () => NotifyReceivers(new LivesUpdatedEvent(--stats.Lives, stats.LivesGained, stats.LivesLost))},
							{SpecialSquareType.BAD_CHANCE_MODIFIER, () => NotifyReceivers(new BadChanceModifierUpdatedEvent(stats.BadChanceModifier -= 0.01))},
							{SpecialSquareType.LIFE, () => NotifyReceivers(new LivesUpdatedEvent(++stats.Lives, stats.LivesGained, stats.LivesLost))},
							{PowerUpSquareType.SOLVER_SMALL, () => NotifyReceivers(new SmallSolversUpdatedEvent(++stats.SmallSolvers))},
							{PowerUpSquareType.SOLVER_MEDIUM, () => NotifyReceivers(new MediumSolversUpdatedEvent(++stats.MediumSolvers))},
							{PowerUpSquareType.SOLVER_LARGE, () => NotifyReceivers(new LargeSolversUpdatedEvent(++stats.LargeSolvers))},
							{PowerUpSquareType.DEFUSER, () => NotifyReceivers(new DefusersUpdatedEvent(++stats.Defusers))}
						});
						NotifyReceivers(new OpenedSquaresUpdatedEvent(++stats.OpenedSquares));
					}
					NotifyReceivers(new SquareUpdatedEvent(position, square, TaskPriority.SQUARE_UPDATE_BASE + @event.Priority));
				}
			});
		}
    }
}