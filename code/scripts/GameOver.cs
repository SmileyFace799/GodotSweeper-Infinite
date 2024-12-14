using Godot;

public partial class GameOver : CanvasLayer {

	public override void _ShortcutInput(InputEvent @event) {
		if (@event is InputEventKey keyEvent) {
			if (keyEvent.Keycode == Key.Escape && keyEvent.IsPressed()) {
				OnBoardGameOver();
			}
			GetViewport().SetInputAsHandled();
		}
	}
	private void OnBoardGameOver() {
		Visible = !Visible;
		GetTree().Paused = Visible;
	}

	public void OnSpectateButtonPressed() {
		OnBoardGameOver();
	}

	public void OnMainMenuButtonPressed() {
		OnBoardGameOver();
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}
}
