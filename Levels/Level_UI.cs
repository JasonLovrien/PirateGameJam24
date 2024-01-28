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

	private int zombies = 0;
	private int adversaries = 0;

	public override void _Ready()
	{
		PlayerHealth = GetNode<ProgressBar>("CanvasLayer/HealthBar");
		_CustomEvents = GetNode<CustomEvents>("/root/CustomEvents");
		_CustomEvents.UpdateZombieCount += UpdateZombieCount;
		_CustomEvents.UpdateSpellName += UpdateSpellName;
		_CustomEvents.UpdatePlayerHealth += UpdatePlayerHealth;
		_CustomEvents.UpdateAdversaryCount += UpdateAdversaryCount;
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
		_CustomEvents.UpdateAdversaryCount -= UpdateAdversaryCount;
	}

	private void UpdateZombieCount(int countChange)
	{
		zombies += countChange;
		ZombieCount.Text = $"Zombies: {zombies}";
	}

	private void UpdateSpellName(string spellName)
	{
		CurrentSpell.Text = $"Selected Spell: {spellName}";
	}

	private void UpdatePlayerHealth(int max, int current)
	{
		PlayerHealth.Value = Mathf.RoundToInt((float)current/(float)max*100);
	}

	private void UpdateAdversaryCount(int count) {
		adversaries += count;
		CurrentSpell.Text = $"Enemies Left: {adversaries}";
	}

	private void UpdatePlayerShield(int max, int current)
	{
		//PlayerShield.Value = Mathf.RoundToInt(current/max*100);
	}
}
