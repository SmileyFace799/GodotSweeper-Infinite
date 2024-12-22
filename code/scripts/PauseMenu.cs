using Godot;

public partial class PauseMenu : CanvasLayer {

	public override void _ShortcutInput(InputEvent @event) {
		if (Visible && @event is InputEventKey keyEvent) {
			if (keyEvent.Keycode == Key.Escape && keyEvent.IsPressed()) {
				OnBoardTogglePaused();
			}
			GetViewport().SetInputAsHandled();
		}
	}
	private void OnBoardTogglePaused() {
		Visible = !Visible;
		GetTree().Paused = Visible;
	}

	public void OnResumeButtonPressed() {
		OnBoardTogglePaused();
	}

	public void OnMainMenuButtonPressed() {
		OnBoardTogglePaused();
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}
}
