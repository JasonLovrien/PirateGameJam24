using Godot;
using System;

public partial class MainMenuBtn : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		GetNode<Timer>("Timer").Timeout += () => {
			Visible = true;
		};
		
		Pressed += () => {
			CallDeferred(nameof(LoadMainMenu));
		};
	}

	private void LoadMainMenu() {
		GetNode("/root/GameOver").QueueFree();
		GetTree().ChangeSceneToFile("res://Menus/MainMenu/MainMenu.tscn");
	}
}
