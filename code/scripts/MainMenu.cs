using Godot;

public partial class MainMenu : MarginContainer {
	public override void _Ready() {
		GetWindow().Title = "RogueSweeper 1.0";
	}

	public void OnPlayButtonPressed() {
		GetTree().ChangeSceneToFile("res://scenes/Board.tscn");
	}

	public void OnQuitButtonPressed() {
		GetTree().Quit();
	}
}
