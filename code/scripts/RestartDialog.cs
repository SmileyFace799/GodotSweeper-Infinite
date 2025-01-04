using Godot;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.Godot
{
	public partial class RestartDialog : CanvasLayer
	{
		private void OnPauseMenuPromptRestart() => Visible = !Visible;
		public static void OnYesButtonPressed() => Game.Instance.Restart();
		public void OnNoButtonPressed() => OnPauseMenuPromptRestart();
	}
}
