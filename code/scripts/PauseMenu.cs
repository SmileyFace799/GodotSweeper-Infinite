using Godot;

namespace SmileyFace799.RogueSweeper.Godot
{
	public partial class PauseMenu : CanvasLayer
	{
		private const string SIGNAL_PROMPT_RESTART = "PromptRestart";

		[Signal]
		public delegate void PromptRestartEventHandler();

		public override void _ShortcutInput(InputEvent @event)
		{
			if (Visible && @event is InputEventKey keyEvent) {
				if (keyEvent.Keycode == Key.Escape && keyEvent.IsPressed()) {
					OnBoardTogglePaused();
				}
				GetViewport().SetInputAsHandled();
			}
		}
		
		private void OnBoardTogglePaused()
		{
			Visible = !Visible;
			GetTree().Paused = Visible;
		}

		public void OnResumeButtonPressed() => OnBoardTogglePaused();
		public void OnRestartButtonPressed() => EmitSignal(SIGNAL_PROMPT_RESTART);
		public void OnMainMenuButtonPressed()
		{
			OnBoardTogglePaused();
			GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
		}
	}
}