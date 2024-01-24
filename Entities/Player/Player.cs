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
	private Godot.Collections.Array<String> AbilityList;
	private List<EntityTag> friendlies = new List<EntityTag>() { EntityTag.Zombie };
	private List<EntityTag> enemis = new List<EntityTag>() { EntityTag.Adversary, EntityTag.Hero };
	private Skillshot skillShot;
	private int AbilityIndex = 0;

	protected override void Initialize()
	{
		GD.Print("spellname: " + AbilityList[0]);
		EntityType = EntityTag.Player;
		//skillShot = GetNode<Skillshot>("Skillshot");
	}

	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("primary_button"))
		{
			GD.Print("In attack");
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
		/*
		if(!skillShot.IsWeaponOnCooldown){
			Spell spell = ResourceLoader.Load<PackedScene>(SpellPath+AbilityList[AbilityIndex]+".tscn").Instantiate() as Spell;
			spell.Initialize(this);
			//skillShot.Attack(GetGlobalMousePosition());
		}*/
	}
}
