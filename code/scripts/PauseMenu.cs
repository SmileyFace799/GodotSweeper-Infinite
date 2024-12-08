using Godot;

public partial class PauseMenu : CanvasLayer {
	private void TogglePause() {
		Visible = !Visible;
		GetTree().Paused = Visible;
	}

    public override void _ShortcutInput(InputEvent @event) {
        base._ShortcutInput(@event);
		if (
			@event is InputEventKey keyEvent
			&& keyEvent.IsPressed()
			&& keyEvent.Keycode == Key.Escape
			&& (keyEvent.GetModifiersMask() & KeyModifierMask.ModifierMask) == 0 // Checks that no keymods are present; it is a pure escape input
		) {
			TogglePause();
			GetViewport().SetInputAsHandled();
		}
    }

	public void OnResumeButtonPressed() {
		TogglePause();
	}
}
