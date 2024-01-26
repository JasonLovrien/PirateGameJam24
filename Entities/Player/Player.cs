using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Player : EntityBase
{
	private CustomEvents _CustomEvents;
	[Export]
	private string SpellPath = "res://Spells/";
	[Export]
	private Godot.Collections.Array<string> AbilityList;
	private Skillshot skillShot;
	private int AbilityIndex = 0;

	protected override void Initialize()
	{
		EntityType = EntityTag.Player;
		_CustomEvents = GetNode<CustomEvents>("/root/CustomEvents");
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

    protected override void ApplyEffect(Effect effect)
    {
        base.ApplyEffect(effect);
		if(effect.EffectedStat.Equals(Stat.CurrentHealth))
		{
			//Update this later to incorporate StatModifiers
			_CustomEvents.EmitSignal(nameof(CustomEvents.UpdatePlayerHealthEventHandler),
			BaseStats[Stat.MaxHealth], BaseStats[Stat.CurrentHealth]);
		}
    }

	public override void Damage(int damage)
	{
		base.Damage(damage);
		//Update this later to incorporate StatModifiers
		_CustomEvents.EmitSignal(nameof(CustomEvents.UpdatePlayerHealthEventHandler), BaseStats[Stat.MaxHealth], BaseStats[Stat.CurrentHealth]);
	}
}
