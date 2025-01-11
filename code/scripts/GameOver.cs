using Godot;
using SmileyFace799.RogueSweeper.events;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.Godot
{
	public partial class GameOver : CanvasLayer, IUIEventReceiver
	{
		public override void _Ready() => Game.Instance.AddReceiver(this);

		public override void _Notification(int what)
        {
			if (what == NotificationPredelete) {
				Game.Instance.RemoveReceiver(this);
			}
        }

		public override void _ShortcutInput(InputEvent @event)
		{
			if (Visible && @event is InputEventKey keyEvent && keyEvent.Keycode == Key.Escape && keyEvent.IsPressed()) {
				OnBoardGameOver();
				GetViewport().SetInputAsHandled();
			}
		}
		private void OnBoardGameOver()
		{
			Visible = !Visible;
			GetTree().Paused = Visible;
		}

		public static void OnNewGameButtonPressed() => Game.Instance.Restart();
		public void OnSpectateButtonPressed() => OnBoardGameOver();
		public void OnMainMenuButtonPressed()
		{
			OnBoardGameOver();
			GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
		}

		public void OnUpdateUI(IUIUpdateEvent @event)
		{
			switch (@event)
			{
				case NewGameLoadedEvent nglEvent:
					if (nglEvent.Stats.Lives <= 0 && !Visible) {
						OnBoardGameOver();
					}
					break;
				case LivesUpdatedEvent luEvent:
					if (luEvent.Lives <= 0 && !Visible) {
						OnBoardGameOver();
					}
					break;
			}
		}

	}
}
