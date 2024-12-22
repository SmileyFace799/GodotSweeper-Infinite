using Godot;
using System;

public partial class SideBarAdvDetails : GridContainer {
	private VBoxContainer _itemsContainer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_itemsContainer = (VBoxContainer) FindChild("ItemsContainer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public void OnExpandButtonToggled(bool toggled) {
		_itemsContainer.Visible = toggled;
	}
}
