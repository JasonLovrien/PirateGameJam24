using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class SquadWithPath : Node2D
{
	[Export]
	public int SquadMembers = 1;

	[Export]
	public int MaxRows = 5;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PathNode NextPathNode = GetNode<NpcPath>("NpcPath").StartNode;
		int column = 0;
		int row = 0;
		for(int i = 0; i < SquadMembers; i++) {
			Adversary adversary = ResourceLoader.Load<PackedScene>("res://Entities/NPCs/Adversary.tscn").Instantiate() as Adversary;
			adversary.NextPathNode = NextPathNode;
			adversary.Position = new Vector2(column * 100, row * 100);
			adversary.PathOffset = adversary.Position;
			AddChild(adversary);

			row++;

			if(row > MaxRows) {
				row = 0;
				column++;
			}
		}
	}
}
