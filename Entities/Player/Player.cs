using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Player : EntityBase
{
	[Export]
	private string SpellPath = "res://Spells/";
	[Export]
	private Godot.Collections.Array<string> AbilityList;
	private Skillshot skillShot;
	private int AbilityIndex = 0;

	protected override void Initialize()
	{
		EntityType = EntityTag.Player;
	}

	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("primary_button"))
		{
			Attack();
		}
	}
	
	protected override Vector2 GetNormalizedMovementDirection()
	{
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		return direction;
	}

	private void Attack() {
		Spell spell = ResourceLoader.Load<PackedScene>(SpellPath+AbilityList[AbilityIndex]+".tscn").Instantiate() as Spell;
		GetTree().Root.AddChild(spell);
		spell.Initialize(this);
	}
}
