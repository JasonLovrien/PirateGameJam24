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
		GD.Print($"HEY, ABOUT TO SET THE PLAYER HEALTH {PlayerHealth}");
		PlayerHealth = GetNode<ProgressBar>("CanvasLayer/HealthBar");
		GD.Print($"HEY, SET THE PLAYER HEALTH {PlayerHealth}");
		_CustomEvents = GetNode<CustomEvents>("/root/CustomEvents");
		_CustomEvents.UpdateZombieCount += UpdateZombieCount;
		_CustomEvents.UpdateSpellName += UpdateSpellName;
		_CustomEvents.UpdatePlayerHealth += UpdatePlayerHealth;
	}

	public override void _Notification(int what)
	{
		base._Notification(what);
		if(what == NotificationPredelete) {
			DisconnectCustomEvents();
		}
	}

	private void DisconnectCustomEvents() {
		_CustomEvents.UpdateZombieCount -= UpdateZombieCount;
		_CustomEvents.UpdateSpellName -= UpdateSpellName;
		_CustomEvents.UpdatePlayerHealth -= UpdatePlayerHealth;
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
		GD.Print($"PlayerHealth baby {PlayerHealth}");
		PlayerHealth.Value = Mathf.RoundToInt((float)current/(float)max*100);
	}

	private void UpdatePlayerShield(int max, int current)
	{
		//PlayerShield.Value = Mathf.RoundToInt(current/max*100);
	}
}
