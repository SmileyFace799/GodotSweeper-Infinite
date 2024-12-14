using Godot;

public partial class MainMenu : MarginContainer {
	public void OnPlayButtonPressed() {
		GetTree().ChangeSceneToFile("res://scenes/Board.tscn");
	}

	public void OnQuitButtonPressed() {
		GetTree().Quit();
	}
}
