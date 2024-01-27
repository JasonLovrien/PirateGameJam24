using Godot;
using System;

public partial class BasicLevel : Node2D
{
	private CustomEvents _CustomEvents;

	private Label levelIndicator;

	private Timer levelIndicatorTimer;

	private int AdversaryCount = 0;

	private int ZombieCount = 0;

	private int level = 1;
	
	public void SetupLevel(int level, int zombiesControlled) {
		GetNode<Player>("Player").GlobalPosition = Vector2.Zero;

		int squads = GetNumberOfSquads(level);
		int enemiesPerSquad = GetEnemiesPerSquad(level);

		int adversaryCount = squads * enemiesPerSquad;
		AdversaryCount = 0;
		_CustomEvents.EmitSignal(CustomEvents.SignalName.UpdateAdversaryCount, adversaryCount);

		levelIndicator.Text = $"LEVEL {level}";
		levelIndicator.Visible = true;
		levelIndicatorTimer.Start();

		SpawnSquads(squads, enemiesPerSquad);
		SpawnZombies(zombiesControlled);
	}
	
	private int GetNumberOfSquads(int level) {
		return (level / 4) + 1;
	}
	
	private int GetEnemiesPerSquad(int level) {
		return 3 + (level * level / 3);
	}

	private void SpawnSquads(int squads, int enemiesPerSquad) {
		for(int i = 0; i < squads; i++) {
			SquadWithPath squad = ResourceLoader.Load<PackedScene>("res://Entities/NPCs/SquadWithPath.tscn").Instantiate() as SquadWithPath;
			squad.SquadMembers = enemiesPerSquad;
			squad.GlobalPosition = new Vector2((i+1) * 1100, 0);
			AddChild(squad);
		}
	}

	private void SpawnZombies(int zombies) {
		int column = 0;
		int row = 0;
		for(int i = 0; i < zombies; i++) {
			ZombieBase zombie = ResourceLoader.Load<PackedScene>("res://Entities/NPCs/ZombieBase.tscn").Instantiate() as ZombieBase;
			zombie.SetThreshhold(50 * (zombies / 5 + 1));
			zombie.GlobalPosition = new Vector2(column * 100, 200 + (row * 100));
			AddChild(zombie);

			row++;

			if(row > 10) {
				row = 0;
				column++;
			}
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_CustomEvents = GetNode<CustomEvents>("/root/CustomEvents");
		_CustomEvents.UpdateAdversaryCount += UpdateAdversaryCount;
		_CustomEvents.UpdateZombieCount += UpdateZombieCount;
		ZombieCount = 0;

		levelIndicator = GetNode<Label>("CanvasLayer/Label");

		levelIndicatorTimer = GetNode<Timer>("LevelStartTimer");

		levelIndicatorTimer.Timeout += () => {levelIndicator.Visible= false;};

		SetupLevel(level, ZombieCount);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	

	public override void _Notification(int what)
	{
		base._Notification(what);
		if(what == NotificationPredelete) {
			DisconnectCustomEvents();
		}
	}

	private void DisconnectCustomEvents() {
		_CustomEvents.UpdateAdversaryCount -= UpdateAdversaryCount;
		_CustomEvents.UpdateZombieCount -= UpdateZombieCount;
	}

	private void UpdateAdversaryCount(int amount) {
		GD.Print($"ADVERSARYCOUNT: {AdversaryCount}");
		AdversaryCount += amount;

		if(AdversaryCount <= 0) {
			CallDeferred(nameof(LevelCompleted));
		}
	}

	private void UpdateZombieCount(int countUpdate) {
		ZombieCount += countUpdate;
	}

	private void LevelCompleted() {
		GD.Print("LEVEL COMPLETED");
		_CustomEvents.EmitSignal(CustomEvents.SignalName.LevelCompleted);
		level += 1;
		SetupLevel(level, ZombieCount);
	}
}
