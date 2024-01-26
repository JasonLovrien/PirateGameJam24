using Godot;
using System;

public partial class Level_UI : Control
{
	private CustomEvents _CustomEvents;
	[Export]
	private Label ZombieCount;
	[Export]
	private Label CurrentSpell;
	[Export]
	private ProgressBar PlayerHealth;
	[Export]
	private ProgressBar PlayerShield;

	public override void _Ready()
    {
		_CustomEvents = GetNode<CustomEvents>("/root/CustomEvents");
		_CustomEvents.UpdateZombieCount += UpdateZombieCount;
		_CustomEvents.UpdateSpellName += UpdateSpellName;
		_CustomEvents.UpdatePlayerHealth += UpdatePlayerHealth;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void UpdateZombieCount(int zombies)
	{
		ZombieCount.Text = $"Zombies: {zombies}";
	}

	private void UpdateSpellName(string spellName)
	{
		CurrentSpell.Text = $"Selected Spell: {spellName}";
	}

	private void UpdatePlayerHealth(int max, int current)
	{
		GD.Print("in hererere");
		PlayerHealth.Value = Mathf.RoundToInt(current/max*100);
	}

	private void UpdatePlayerShield(int max, int current)
	{
		//PlayerShield.Value = Mathf.RoundToInt(current/max*100);
	}
}