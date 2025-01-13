using Godot;
using SmileyFace799.RogueSweeper.events;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.Godot {
	public partial class SideBarPowerUps : MarginContainer, IUIEventReceiver
	{
		private Label _solversSmallLabel;
		private Label _solversMediumLabel;
		private Label _solversLargeLabel;
		private Label _defusersLabel;

		private TextureButton _selectedButton;
		private TextureButton _solversSmallButton;
		private TextureButton _solversMediumButton;
		private TextureButton _solversLargeButton;
		private TextureButton _defusersButton;

		public override void _Ready()
		{
			_solversSmallLabel = (Label) FindChild("SolverSmallValue");
			_solversMediumLabel = (Label) FindChild("SolverMediumValue");
			_solversLargeLabel = (Label) FindChild("SolverLargeValue");
			_defusersLabel = (Label) FindChild("DefuseValue");

			_solversSmallButton = (TextureButton) FindChild("SolverSmallButton");
			_solversMediumButton = (TextureButton) FindChild("SolverMediumButton");
			_solversLargeButton = (TextureButton) FindChild("SolverLargeButton");
			_defusersButton = (TextureButton) FindChild("DefuseButton");
			Game.Instance.AddReceiver(this);
		}

		public override void _Notification(int what)
        {
			if (what == NotificationPredelete) {
				Game.Instance.RemoveReceiver(this);
			}
        }

		private void PowerUpSelectionToggled(bool on, PowerUp powerUp, TextureButton button)
		{
			if (on) {
				Game.Instance.SelectedPowerUp = powerUp;
				_selectedButton = button;
			} else {
				Game.Instance.SelectedPowerUp = null;
				_selectedButton = null;
			}
		}

		public void OnSolverSmallButtonToggled(bool on) => PowerUpSelectionToggled(on, PowerUp.SOLVER_SMALL, _solversSmallButton);
		public void OnSolverMediumButtonToggled(bool on) => PowerUpSelectionToggled(on, PowerUp.SOLVER_MEDIUM, _solversMediumButton);
		public void OnSolverLargeButtonToggled(bool on) => PowerUpSelectionToggled(on, PowerUp.SOLVER_LARGE, _solversLargeButton);
		public void OnDefuseButtonToggled(bool on) => PowerUpSelectionToggled(on, PowerUp.DEFUSER, _defusersButton);

		private void OnSmallSolversUpdated(uint smallSolvers)
		{
			GDThread.QueueTask(TaskPriority.UI_UPDATE, () => {
				_solversSmallButton.Disabled = smallSolvers == 0;
				_solversSmallLabel.Text = smallSolvers.ToString();
			});
		}
		private void OnMediumSolversUpdated(uint mediumSolvers)
		{
			GDThread.QueueTask(TaskPriority.UI_UPDATE, () => {
				_solversMediumButton.Disabled = mediumSolvers == 0;
				_solversMediumLabel.Text = mediumSolvers.ToString();
			});
		}
		private void OnLargeSolversUpdated(uint largeSolvers)
		{
			GDThread.QueueTask(TaskPriority.UI_UPDATE, () => {
				_solversLargeButton.Disabled = largeSolvers == 0;
				_solversLargeLabel.Text = largeSolvers.ToString();
			});
		}
		private void OnDefusersUpdated(uint defusers)
		{
			GDThread.QueueTask(TaskPriority.UI_UPDATE, () => {
				_defusersButton.Disabled = defusers == 0;
				_defusersLabel.Text = defusers.ToString();
			});
		}

		public void OnUpdateUI(IUIUpdateEvent @event)
		{
			switch (@event) {
				case NewGameLoadedEvent nglEvent:
					OnSmallSolversUpdated(nglEvent.Stats.SmallSolvers);
					OnMediumSolversUpdated(nglEvent.Stats.MediumSolvers);
					OnLargeSolversUpdated(nglEvent.Stats.LargeSolvers);
					OnDefusersUpdated(nglEvent.Stats.Defusers);
					break;
				case SmallSolversUpdatedEvent ssuEvent:
					OnSmallSolversUpdated(ssuEvent.SmallSolvers);
					break;
				case MediumSolversUpdatedEvent msuEvent:
					OnMediumSolversUpdated(msuEvent.MediumSolvers);
					break;
				case LargeSolversUpdatedEvent lsuEvent:
					OnLargeSolversUpdated(lsuEvent.LargeSolvers);
					break;
				case DefusersUpdatedEvent duEvent:
					OnDefusersUpdated(duEvent.Defusers);
					break;
				case PowerUpDeselectedEvent:
					GDThread.QueueTask(TaskPriority.UI_UPDATE, () => {
						if (_selectedButton != null) {
							_selectedButton.SetPressedNoSignal(false);
						}
						_selectedButton = null;
					});
					break;
			}
		}
	}
}
