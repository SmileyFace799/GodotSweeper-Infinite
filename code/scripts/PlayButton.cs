using Godot;

public partial class PlayButton : Button {
	public void OnPressed() {
		GetTree().ChangeSceneToFile("res://scenes/Board.tscn");
	}
}
