using Godot;
using System;

public partial class BasicLevel : Node2D
{
	
	public void SetupLevel(int level, int zombiesControlled) {
		GetNode<Player>("Player").GlobalPosition = Vector2.Zero;

		int squads = GetNumberOfSquads(level);
		int enemiesPerSquad = GetEnemiesPerSquad(level);

		SpawnSquads(squads, enemiesPerSquad);
		SpawnZombies(zombiesControlled);
	}
	
	private int GetNumberOfSquads(int level) {
		return 1;
		//return (level / 4) + 1;
	}
	
	private int GetEnemiesPerSquad(int level) {
		return 1;
		//return 3 + (level * level / 3);
	}

	private void SpawnSquads(int squads, int enemiesPerSquad) {
		for(int i = 0; i < squads; i++) {
			SquadWithPath squad = ResourceLoader.Load<PackedScene>("res://Entities/NPCs/SquadWithPath.tscn").Instantiate() as SquadWithPath;
			squad.SquadMembers = enemiesPerSquad;
			squad.GlobalPosition = new Vector2((i+1) * 1500, 0);
			AddChild(squad);
		}
	}

	private void SpawnZombies(int zombies) {
		int column = 0;
		int row = 0;
		for(int i = 0; i < zombies; i++) {
			ZombieBase zombie = ResourceLoader.Load<PackedScene>("res://Entities/NPCs/ZombieBase.tscn").Instantiate() as ZombieBase;
			zombie.SetThreshhold(50 * (zombies / 5 + 1));
			zombie.GlobalPosition = new Vector2(column * 200, (row + 1) * 200);
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
		SetupLevel(2, 1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
