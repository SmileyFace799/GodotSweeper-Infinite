using Godot;

namespace SmileyFace799.RogueSweeper.Godot
{
	public partial class Global : Node
	{
		public override void _ShortcutInput(InputEvent @event)
		{
			if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.F11) {
				if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen) {
					DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
					DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				} else {
					DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
					DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				}
				GetViewport().SetInputAsHandled();
			}
		}
	}
}
