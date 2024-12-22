using Godot;
using System;

public partial class SideBar : CanvasLayer {
	private Label _openedSquaresLabel;
	private Label _livesLabel;
	private Label _badChanceLabel;
	private Label _goodChanceLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_openedSquaresLabel = (Label) FindChild("OpenedSquaresValue");
		_livesLabel = (Label) FindChild("LivesValue");
		_badChanceLabel = (Label) FindChild("BadChanceValue");
		_goodChanceLabel = (Label) FindChild("GoodChanceValue");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public void OnBoardSquaresOpenedUpdated(ulong squaresOpened) {
		_openedSquaresLabel.Text = squaresOpened.ToString();
	}

	public void OnBoardLivesUpdated(int lives) {
		_livesLabel.Text = lives.ToString();
	}

	public void OnBoardHoveredSquareUpdated(long x, long y, double badChanceReduction) {
		BoardModel.SquareGenData genData = new BoardModel.RelativeGenData(badChanceReduction);
		_badChanceLabel.Text = Math.Clamp(genData.GetBadChance(new(x, y)), 0, 1).ToString("P");
		_goodChanceLabel.Text = Math.Clamp(genData.GetGoodChance(new(x, y)), 0, 1).ToString("P");
	}
}
