using Godot;
using SmileyFace799.RogueSweeper.events;
using SmileyFace799.RogueSweeper.model;
using System;

namespace SmileyFace799.RogueSweeper.Godot
{
	public partial class SideBar : CanvasLayer, IUIEventReceiver
	{
		private Label _openedSquaresLabel;
		private Label _livesLabel;
		private Label _badChanceLabel;
		private Label _goodChanceLabel;

		private Label _extraLivesLabel;
		private Label _badChanceModifierLabel;
		

		private Position _hoveredSquare = new(0, 0);
		private Position HoveredSquare {get => _hoveredSquare; set {
			_hoveredSquare = value;
			UpdateHoveredInfo();
		}}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_openedSquaresLabel = (Label) FindChild("OpenedSquaresValue");
			_livesLabel = (Label) FindChild("LivesValue");
			_badChanceLabel = (Label) FindChild("BadChanceValue");
			_goodChanceLabel = (Label) FindChild("GoodChanceValue");
			_extraLivesLabel = (Label) FindChild("ExtraLivesValue");
			_badChanceModifierLabel = (Label) FindChild("BadChanceModifierValue");

			Game.Instance.AddReceiver(this);
		}

		public override void _Notification(int what)
        {
			if (what == NotificationPredelete) {
				Game.Instance.RemoveReceiver(this);
			}
        }

		public void OnBoardHoveredSquareUpdated(int x, int y) => HoveredSquare = new(x, y);

		private void UpdateHoveredInfo()
		{
			SquareGenData genData = Game.Instance.StandardGenData;
			GDThread.QueueTask(TaskPriority.UI_UPDATE, () => {
				_badChanceLabel.Text = Math.Clamp(genData.GetBadChance(HoveredSquare), 0, 1).ToString("P");
				_goodChanceLabel.Text = Math.Clamp(genData.GetGoodChance(HoveredSquare), 0, 1).ToString("P");
			});
		}

		private void OnLivesUpdated(int lives, int livesGained)
		{
			GDThread.QueueTask(TaskPriority.UI_UPDATE, () => {
				_livesLabel.Text = lives.ToString();
				_extraLivesLabel.Text = livesGained.ToString();
			});
		}

		private void OnBadChanceModifierUpdated(double badChanceModifier) {
			GDThread.QueueTask(TaskPriority.UI_UPDATE, () => _badChanceModifierLabel.Text = badChanceModifier.ToString("P"));
			UpdateHoveredInfo();
		}

		private void OnOpenedSquaresUpdated(ulong squaresOpened) => GDThread.QueueTask(TaskPriority.UI_UPDATE, () => _openedSquaresLabel.Text = squaresOpened.ToString());

		public void OnUpdateUI(IUIUpdateEvent @event)
		{
			switch (@event) {
				case NewGameLoadedEvent nglEvent:
					OnLivesUpdated(nglEvent.Stats.Lives, nglEvent.Stats.LivesGained);
					OnBadChanceModifierUpdated(nglEvent.Stats.BadChanceModifier);
					OnOpenedSquaresUpdated(nglEvent.Stats.OpenedSquares);
					break;
				case LivesUpdatedEvent luEvent:
					OnLivesUpdated(luEvent.Lives, luEvent.LivesGained);
					break;
				case BadChanceModifierUpdatedEvent bcmuEvent:
					OnBadChanceModifierUpdated(bcmuEvent.BadChanceModifier);
					break;
				case OpenedSquaresUpdatedEvent osuEvent:
					OnOpenedSquaresUpdated(osuEvent.OpenedSquares);
					break;
			}
		}
	}
}